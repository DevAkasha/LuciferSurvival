using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Utility class to track memory usage and subscription relationships in Rx objects.
/// Helps detect memory leaks and circular dependencies.
/// </summary>
public static class RxMemoryTracker
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD || RXDEBUG
    private class RxObjectInfo
    {
        public string Context;
        public System.WeakReference WeakRef;
        public HashSet<object> Subscribers;
        public Type Type;
        public DateTime CreationTime;

        public RxObjectInfo(object obj, string context)
        {
            WeakRef = new System.WeakReference(obj);
            Context = context;
            Type = obj.GetType();
            Subscribers = new HashSet<object>();
            CreationTime = DateTime.Now;
        }

        public bool IsAlive => WeakRef.IsAlive;
    }

    private static readonly List<RxObjectInfo> trackedObjects = new();
    private static readonly Dictionary<object, HashSet<RxBase>> subscriptions = new();
    private static bool isTracking = false;
    private static bool autoTrackNew = false;
    private static DateTime trackingStart;

    /// <summary>
    /// Enable memory tracking for Rx objects
    /// </summary>
    /// <param name="trackNewObjects">Automatically track new objects as they're created</param>
    public static void Enable(bool trackNewObjects = false)
    {
        isTracking = true;
        autoTrackNew = trackNewObjects;
        trackingStart = DateTime.Now;
        Debug.Log($"[RxMemoryTracker] Enabled memory tracking (Auto-track new: {autoTrackNew})");
    }

    /// <summary>
    /// Disable memory tracking
    /// </summary>
    public static void Disable()
    {
        isTracking = false;
        Debug.Log($"[RxMemoryTracker] Disabled memory tracking");
    }

    /// <summary>
    /// Track a new Rx object
    /// </summary>
    public static void TrackObject(object obj, string context = null)
    {
        if (!isTracking) return;

        if (obj == null)
        {
            Debug.LogWarning("[RxMemoryTracker] Attempted to track null object");
            return;
        }

        // Check that we're not already tracking this object
        foreach (var info in trackedObjects)
        {
            if (info.IsAlive && info.WeakRef.Target == obj)
            {
                // Already tracking, update context if provided
                if (!string.IsNullOrEmpty(context))
                {
                    info.Context = context;
                }
                return;
            }
        }

        // Add new tracked object
        trackedObjects.Add(new RxObjectInfo(obj, context ?? $"Unknown-{obj.GetType().Name}"));
    }

    /// <summary>
    /// Record a subscription relationship between an Rx publisher and a subscriber
    /// </summary>
    public static void TrackSubscription(RxBase publisher, object subscriber)
    {
        if (!isTracking || publisher == null || subscriber == null) return;

        // Find the tracked publisher
        RxObjectInfo publisherInfo = null;
        foreach (var info in trackedObjects)
        {
            if (info.IsAlive && info.WeakRef.Target == publisher)
            {
                publisherInfo = info;
                break;
            }
        }

        // If we're auto-tracking new objects, add the publisher if not found
        if (publisherInfo == null && autoTrackNew)
        {
            publisherInfo = new RxObjectInfo(publisher, $"Auto-tracked-{publisher.GetType().Name}");
            trackedObjects.Add(publisherInfo);
        }

        // Cannot record subscription for unknown publisher
        if (publisherInfo == null) return;

        // Record the subscription
        publisherInfo.Subscribers.Add(subscriber);

        // Also record reverse lookup for the subscriber
        if (!subscriptions.TryGetValue(subscriber, out var publisherSet))
        {
            publisherSet = new HashSet<RxBase>();
            subscriptions[subscriber] = publisherSet;
        }
        publisherSet.Add(publisher);
    }

    /// <summary>
    /// Check for memory leaks in tracked objects
    /// </summary>
    public static void DumpLeaks()
    {
        if (!isTracking || trackedObjects.Count == 0)
        {
            Debug.Log("[RxMemoryTracker] No objects being tracked");
            return;
        }

        // Force garbage collection
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        int aliveCount = 0;
        int leakCount = 0;
        StringBuilder report = new StringBuilder();
        report.AppendLine($"[RxMemoryTracker] Memory Report - {trackedObjects.Count} tracked objects");
        report.AppendLine($"Tracking started at: {trackingStart}");

        Dictionary<Type, int> typeCounts = new Dictionary<Type, int>();

        for (int i = trackedObjects.Count - 1; i >= 0; i--)
        {
            var info = trackedObjects[i];

            if (info.IsAlive)
            {
                aliveCount++;

                // Count by type
                if (!typeCounts.ContainsKey(info.Type))
                    typeCounts[info.Type] = 0;

                typeCounts[info.Type]++;

                // Check for potential leaks (objects that have been alive for a long time)
                TimeSpan age = DateTime.Now - info.CreationTime;
                if (age.TotalMinutes > 1) // Adjust threshold as needed
                {
                    leakCount++;
                    report.AppendLine($"Potential leak: {info.Type.Name} created {age.TotalSeconds:F1}s ago in context \"{info.Context}\"");

                    // Show subscriber count
                    if (info.Subscribers.Count > 0)
                    {
                        report.AppendLine($"  Has {info.Subscribers.Count} subscribers");
                    }
                }
            }
            else
            {
                // Object has been GC'd, remove from tracking
                trackedObjects.RemoveAt(i);
            }
        }

        // Report on type distribution
        report.AppendLine($"\nActive objects by type:");
        foreach (var typeCount in typeCounts.OrderByDescending(kvp => kvp.Value))
        {
            report.AppendLine($"  {typeCount.Key.Name}: {typeCount.Value}");
        }

        report.AppendLine($"\nSummary: {aliveCount} alive, {leakCount} potential leaks, {trackedObjects.Count - aliveCount} collected");

        if (leakCount > 0)
        {
            Debug.LogWarning(report.ToString());
        }
        else
        {
            Debug.Log(report.ToString());
        }

        // Clean up subscription dictionary, removing entries for collected objects
        CleanupSubscriptions();
    }

    /// <summary>
    /// Clean up subscriptions dictionary
    /// </summary>
    private static void CleanupSubscriptions()
    {
        List<object> toRemove = new List<object>();

        foreach (var kvp in subscriptions)
        {
            // Check if subscriber is a weak reference
            if (kvp.Key is WeakReference weakRef)
            {
                if (!weakRef.IsAlive)
                {
                    toRemove.Add(kvp.Key);
                }
            }

            // Remove any publishers that have been garbage collected
            kvp.Value.RemoveWhere(publisher => !IsTrackedObjectAlive(publisher));

            // If no publishers left, remove the subscriber entry
            if (kvp.Value.Count == 0)
            {
                toRemove.Add(kvp.Key);
            }
        }

        // Remove dead subscribers
        foreach (var key in toRemove)
        {
            subscriptions.Remove(key);
        }
    }

    /// <summary>
    /// Check if a tracked object is still alive
    /// </summary>
    private static bool IsTrackedObjectAlive(object obj)
    {
        foreach (var info in trackedObjects)
        {
            if (info.IsAlive && info.WeakRef.Target == obj)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Detect circular subscriptions
    /// </summary>
    public static void DetectCyclicSubscriptions()
    {
        if (!isTracking)
        {
            Debug.Log("[RxMemoryTracker] Memory tracking is disabled");
            return;
        }

        // Clean up subscriptions first
        CleanupSubscriptions();

        // Build subscription graph
        Dictionary<object, HashSet<object>> graph = new Dictionary<object, HashSet<object>>();

        // Convert our subscriptions to a simple graph
        foreach (var trackedObj in trackedObjects)
        {
            if (!trackedObj.IsAlive) continue;

            object publisher = trackedObj.WeakRef.Target;
            if (!graph.ContainsKey(publisher))
            {
                graph[publisher] = new HashSet<object>();
            }

            foreach (var subscriber in trackedObj.Subscribers)
            {
                if (IsTrackedObjectAlive(subscriber))
                {
                    graph[publisher].Add(subscriber);
                }
            }
        }

        // Find cycles
        HashSet<(object, object)> cycles = new HashSet<(object, object)>();
        foreach (var node in graph.Keys)
        {
            FindCycles(node, node, new HashSet<object>(), graph, cycles);
        }

        // Report cycles
        if (cycles.Count > 0)
        {
            StringBuilder report = new StringBuilder();
            report.AppendLine($"[RxMemoryTracker] Found {cycles.Count} cyclic dependencies:");

            foreach (var cycle in cycles)
            {
                string type1 = cycle.Item1?.GetType().Name ?? "null";
                string type2 = cycle.Item2?.GetType().Name ?? "null";
                report.AppendLine($"  Cycle between {type1} and {type2}");
            }

            Debug.LogWarning(report.ToString());
        }
        else
        {
            Debug.Log("[RxMemoryTracker] No cyclic dependencies found");
        }
    }

    /// <summary>
    /// Find cycles in the subscription graph using depth-first search
    /// </summary>
    private static void FindCycles(object start, object current, HashSet<object> visited,
                                  Dictionary<object, HashSet<object>> graph, HashSet<(object, object)> cycles)
    {
        if (visited.Contains(current))
            return;

        visited.Add(current);

        if (!graph.TryGetValue(current, out var neighbors))
            return;

        foreach (var neighbor in neighbors)
        {
            // Found a cycle back to the start
            if (neighbor.Equals(start))
            {
                cycles.Add((current, start));
                continue;
            }

            // Continue DFS if neighbor is not start
            if (!visited.Contains(neighbor))
            {
                FindCycles(start, neighbor, new HashSet<object>(visited), graph, cycles);
            }
        }
    }
#else
    // Stub implementations for release builds
    public static void Enable(bool trackNewObjects = false) { }
    public static void Disable() { }
    public static void TrackObject(object obj, string context = null) { }
    public static void TrackSubscription(RxBase publisher, object subscriber) { }
    public static void DumpLeaks() { }
    public static void DetectCyclicSubscriptions() { }
#endif
}