using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Monitors and reports performance metrics for Rx operations
/// Tracks notification duration, frequency, and memory usage
/// </summary>
public static class RxPerformanceMonitor
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD || RXDEBUG
    // 클래스의 접근성을 public으로 변경 (internal도 가능)
    public class NotificationMetrics
    {
        public int Count;
        public double TotalDuration;
        public double MaxDuration;
        public double LastDuration;
        public int ListenerCount;
        public DateTime LastUpdate;
        public DateTime FirstUpdate;
        public List<double> RecentDurations;

        public NotificationMetrics()
        {
            Count = 0;
            TotalDuration = 0;
            MaxDuration = 0;
            LastDuration = 0;
            ListenerCount = 0;
            RecentDurations = new List<double>(100); // Store last 100 durations
            LastUpdate = DateTime.Now;
            FirstUpdate = DateTime.Now;
        }

        public double AverageDuration => Count > 0 ? TotalDuration / Count : 0;

        public double MedianDuration
        {
            get
            {
                if (RecentDurations.Count == 0) return 0;

                var sorted = RecentDurations.OrderBy(d => d).ToList();
                int mid = sorted.Count / 2;

                if (sorted.Count % 2 == 0)
                    return (sorted[mid] + sorted[mid - 1]) / 2;
                else
                    return sorted[mid];
            }
        }
    }

    private static bool isEnabled = false;
    private static bool detailedLogging = false;
    private static double slowNotificationThreshold = 5.0; // ms

    private static readonly Dictionary<object, NotificationMetrics> objectMetrics = new();
    private static readonly Dictionary<Type, NotificationMetrics> typeMetrics = new();
    private static readonly List<(DateTime, Type, double, int)> recentSlowNotifications = new(); // time, type, duration, listeners
    private static readonly Stopwatch measurementTimer = new Stopwatch();

    private static long totalMemoryBaseline = 0;
    private static DateTime lastMemoryCheck = DateTime.MinValue;
    private static readonly TimeSpan memoryCheckInterval = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Enable performance monitoring
    /// </summary>
    /// <param name="detailed">Whether to log detailed information for every notification</param>
    public static void Enable(bool detailed = false)
    {
        isEnabled = true;
        detailedLogging = detailed;

        // Capture baseline memory usage
        GC.Collect();
        totalMemoryBaseline = GC.GetTotalMemory(true);
        lastMemoryCheck = DateTime.Now;

        UnityEngine.Debug.Log($"[RxPerformanceMonitor] Enabled (Detailed logging: {detailedLogging})");
        UnityEngine.Debug.Log($"[RxPerformanceMonitor] Memory baseline: {FormatMemory(totalMemoryBaseline)}");
    }

    /// <summary>
    /// Disable performance monitoring
    /// </summary>
    public static void Disable()
    {
        isEnabled = false;
        UnityEngine.Debug.Log("[RxPerformanceMonitor] Disabled");
    }

    /// <summary>
    /// Start a performance measurement
    /// </summary>
    public static void StartMeasurement()
    {
        if (!isEnabled) return;
        measurementTimer.Restart();
    }

    /// <summary>
    /// End a performance measurement and record metrics for an object
    /// </summary>
    /// <param name="obj">The object being monitored</param>
    /// <param name="listenerCount">Number of listeners that received the notification</param>
    public static void EndMeasurement(object obj, int listenerCount)
    {
        if (!isEnabled || obj == null) return;

        double elapsedMs = measurementTimer.Elapsed.TotalMilliseconds;
        Type objType = obj.GetType();

        // Record per-object metrics
        if (!objectMetrics.TryGetValue(obj, out var metrics))
        {
            metrics = new NotificationMetrics();
            objectMetrics[obj] = metrics;
        }

        UpdateMetrics(metrics, elapsedMs, listenerCount);

        // Record per-type metrics
        if (!typeMetrics.TryGetValue(objType, out var typeMetric))
        {
            typeMetric = new NotificationMetrics();
            typeMetrics[objType] = typeMetric;
        }

        UpdateMetrics(typeMetric, elapsedMs, listenerCount);

        // Check for slow notifications
        if (elapsedMs > slowNotificationThreshold)
        {
            recentSlowNotifications.Add((DateTime.Now, objType, elapsedMs, listenerCount));

            // Keep the list at a reasonable size
            if (recentSlowNotifications.Count > 100)
            {
                recentSlowNotifications.RemoveAt(0);
            }

            UnityEngine.Debug.LogWarning($"[RxPerformanceMonitor] Slow notification detected: {objType.Name} took {elapsedMs:F2}ms with {listenerCount} listeners");
        }

        // Detailed logging if enabled
        if (detailedLogging)
        {
            UnityEngine.Debug.Log($"[RxPerformanceMonitor] {objType.Name} notification: {elapsedMs:F2}ms with {listenerCount} listeners");
        }

        // Periodically check memory usage
        if ((DateTime.Now - lastMemoryCheck) > memoryCheckInterval)
        {
            CheckMemoryUsage();
            lastMemoryCheck = DateTime.Now;
        }
    }

    /// <summary>
    /// Get performance metrics for a specific object
    /// </summary>
    public static string GetObjectMetrics(object obj)
    {
        if (!isEnabled || obj == null || !objectMetrics.TryGetValue(obj, out var metrics))
        {
            return "No metrics available";
        }

        return FormatMetrics(obj.GetType().Name, metrics);
    }

    /// <summary>
    /// Get performance metrics for a specific type
    /// </summary>
    public static string GetTypeMetrics(Type type)
    {
        if (!isEnabled || type == null || !typeMetrics.TryGetValue(type, out var metrics))
        {
            return "No metrics available";
        }

        return FormatMetrics(type.Name, metrics);
    }

    /// <summary>
    /// Generate a comprehensive performance report
    /// </summary>
    public static string GenerateReport()
    {
        if (!isEnabled)
        {
            return "Performance monitoring is disabled";
        }

        StringBuilder report = new StringBuilder();
        report.AppendLine("=== Rx Performance Report ===");
        report.AppendLine($"Time: {DateTime.Now}");
        report.AppendLine($"Tracking {objectMetrics.Count} objects of {typeMetrics.Count} types");
        report.AppendLine();

        // Memory usage
        CheckMemoryUsage(false);
        long currentMemory = GC.GetTotalMemory(false);
        long memoryDiff = currentMemory - totalMemoryBaseline;

        report.AppendLine("Memory Usage:");
        report.AppendLine($"  Baseline: {FormatMemory(totalMemoryBaseline)}");
        report.AppendLine($"  Current: {FormatMemory(currentMemory)}");
        report.AppendLine($"  Change: {FormatMemory(memoryDiff)} ({(memoryDiff > 0 ? "+" : "")}{memoryDiff * 100.0 / totalMemoryBaseline:F1}%)");
        report.AppendLine();

        // Top types by total time
        report.AppendLine("Top Types by Total Notification Time:");
        var topTypes = typeMetrics
            .OrderByDescending(kvp => kvp.Value.TotalDuration)
            .Take(10);

        foreach (var type in topTypes)
        {
            report.AppendLine($"  {type.Key.Name}:");
            report.AppendLine($"    Count: {type.Value.Count}");
            report.AppendLine($"    Total: {type.Value.TotalDuration:F2}ms");
            report.AppendLine($"    Avg: {type.Value.AverageDuration:F3}ms");
            report.AppendLine($"    Max: {type.Value.MaxDuration:F2}ms");
            report.AppendLine($"    Listeners: {type.Value.ListenerCount}");
        }
        report.AppendLine();

        // Recent slow notifications
        if (recentSlowNotifications.Count > 0)
        {
            report.AppendLine("Recent Slow Notifications:");
            foreach (var (time, type, duration, listeners) in recentSlowNotifications
                .OrderByDescending(n => n.Item3)
                .Take(5))
            {
                report.AppendLine($"  {type.Name}: {duration:F2}ms with {listeners} listeners at {time:HH:mm:ss}");
            }
            report.AppendLine();
        }

        // Objects with high listener counts
        report.AppendLine("Objects with High Listener Counts:");
        var highListenerObjects = objectMetrics
            .OrderByDescending(kvp => kvp.Value.ListenerCount)
            .Take(5);

        foreach (var obj in highListenerObjects)
        {
            report.AppendLine($"  {obj.Key.GetType().Name}: {obj.Value.ListenerCount} listeners");
        }

        return report.ToString();
    }

    /// <summary>
    /// Reset all recorded metrics
    /// </summary>
    public static void Reset()
    {
        objectMetrics.Clear();
        typeMetrics.Clear();
        recentSlowNotifications.Clear();

        // Capture new baseline memory usage
        GC.Collect();
        totalMemoryBaseline = GC.GetTotalMemory(true);
        lastMemoryCheck = DateTime.Now;

        UnityEngine.Debug.Log($"[RxPerformanceMonitor] Reset all metrics (New memory baseline: {FormatMemory(totalMemoryBaseline)})");
    }

    /// <summary>
    /// Set the threshold for slow notification warnings
    /// </summary>
    /// <param name="thresholdMs">Threshold in milliseconds</param>
    public static void SetSlowNotificationThreshold(double thresholdMs)
    {
        slowNotificationThreshold = thresholdMs;
        UnityEngine.Debug.Log($"[RxPerformanceMonitor] Slow notification threshold set to {thresholdMs}ms");
    }

    // Helper methods

    private static void UpdateMetrics(NotificationMetrics metrics, double duration, int listenerCount)
    {
        metrics.Count++;
        metrics.TotalDuration += duration;
        metrics.MaxDuration = Math.Max(metrics.MaxDuration, duration);
        metrics.LastDuration = duration;
        metrics.ListenerCount = listenerCount; // Update to most recent count
        metrics.LastUpdate = DateTime.Now;

        // Store recent durations for median calculation
        metrics.RecentDurations.Add(duration);
        if (metrics.RecentDurations.Count > 100) // Keep last 100 measurements
        {
            metrics.RecentDurations.RemoveAt(0);
        }
    }

    private static string FormatMetrics(string name, NotificationMetrics metrics)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"=== Metrics for {name} ===");
        sb.AppendLine($"Total notifications: {metrics.Count}");
        sb.AppendLine($"First update: {metrics.FirstUpdate}");
        sb.AppendLine($"Last update: {metrics.LastUpdate}");
        sb.AppendLine($"Total duration: {metrics.TotalDuration:F2}ms");
        sb.AppendLine($"Average duration: {metrics.AverageDuration:F3}ms");
        sb.AppendLine($"Median duration: {metrics.MedianDuration:F3}ms");
        sb.AppendLine($"Maximum duration: {metrics.MaxDuration:F2}ms");
        sb.AppendLine($"Last duration: {metrics.LastDuration:F2}ms");
        sb.AppendLine($"Listener count: {metrics.ListenerCount}");

        return sb.ToString();
    }

    private static void CheckMemoryUsage(bool logToConsole = true)
    {
        long currentMemory = GC.GetTotalMemory(false);
        long memoryDiff = currentMemory - totalMemoryBaseline;

        if (logToConsole)
        {
            UnityEngine.Debug.Log($"[RxPerformanceMonitor] Memory usage: {FormatMemory(currentMemory)} (Change: {FormatMemory(memoryDiff)})");
        }
    }

    private static string FormatMemory(long bytes)
    {
        if (bytes < 1024)
            return $"{bytes} B";
        else if (bytes < 1024 * 1024)
            return $"{bytes / 1024.0:F2} KB";
        else if (bytes < 1024 * 1024 * 1024)
            return $"{bytes / (1024.0 * 1024.0):F2} MB";
        else
            return $"{bytes / (1024.0 * 1024.0 * 1024.0):F2} GB";
    }

    /// <summary>
    /// Remove object from metrics tracking (use when object is disposed)
    /// </summary>
    public static void RemoveObjectTracking(object obj)
    {
        if (!isEnabled || obj == null) return;
        objectMetrics.Remove(obj);
    }

    /// <summary>
    /// Get types ordered by specific metric
    /// </summary>
    public static List<(Type Type, double Value)> GetTypesByMetric(Func<NotificationMetrics, double> metricSelector)
    {
        return typeMetrics
            .OrderByDescending(kvp => metricSelector(kvp.Value))
            .Select(kvp => (kvp.Key, metricSelector(kvp.Value)))
            .ToList();
    }
#else
    // Stub implementation to avoid the NotificationMetrics class not found error
    public class NotificationMetrics { }

    // Stub implementations for release builds
    public static void Enable(bool detailed = false) { }
    public static void Disable() { }
    public static void StartMeasurement() { }
    public static void EndMeasurement(object obj, int listenerCount) { }
    public static string GetObjectMetrics(object obj) { return string.Empty; }
    public static string GetTypeMetrics(Type type) { return string.Empty; }
    public static string GenerateReport() { return string.Empty; }
    public static void Reset() { }
    public static void SetSlowNotificationThreshold(double thresholdMs) { }
    public static void RemoveObjectTracking(object obj) { }
    public static List<(Type Type, double Value)> GetTypesByMetric(Func<NotificationMetrics, double> metricSelector) { return new List<(Type Type, double Value)>(); }
#endif
}

#if UNITY_EDITOR
/// <summary>
/// Editor window for RxPerformanceMonitor visualization and control
/// </summary>
public class RxPerformanceMonitorWindow : EditorWindow
{
    private Vector2 scrollPosition;
    private bool autoRefresh = true;
    private bool detailedLogging = false;
    private double slowThreshold = 5.0;
    private string reportText = "Enable monitoring to see performance metrics";
    private double refreshInterval = 2.0; // seconds
    private double lastRefreshTime = 0.0;

    [MenuItem("Tools/Rx Framework/Performance Monitor")]
    public static void ShowWindow()
    {
        GetWindow<RxPerformanceMonitorWindow>("Rx Performance");
    }

    private void OnEnable()
    {
        // Load saved preferences
        autoRefresh = EditorPrefs.GetBool("RxPerformanceMonitor.AutoRefresh", true);
        detailedLogging = EditorPrefs.GetBool("RxPerformanceMonitor.DetailedLogging", false);
        slowThreshold = EditorPrefs.GetFloat("RxPerformanceMonitor.SlowThreshold", 5.0f);
    }

    private void OnDisable()
    {
        // Save preferences
        EditorPrefs.SetBool("RxPerformanceMonitor.AutoRefresh", autoRefresh);
        EditorPrefs.SetBool("RxPerformanceMonitor.DetailedLogging", detailedLogging);
        EditorPrefs.SetFloat("RxPerformanceMonitor.SlowThreshold", (float)slowThreshold);
    }

    private void Update()
    {
        if (autoRefresh && EditorApplication.isPlaying)
        {
            double currentTime = EditorApplication.timeSinceStartup;
            if (currentTime - lastRefreshTime > refreshInterval)
            {
                RefreshReport();
                lastRefreshTime = currentTime;
                Repaint();
            }
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.LabelField("Rx Performance Monitor", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Monitors performance of reactive notifications", MessageType.Info);

        EditorGUILayout.Space();

        // Settings
        EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);

        bool newDetailedLogging = EditorGUILayout.Toggle("Detailed Logging", detailedLogging);
        if (newDetailedLogging != detailedLogging)
        {
            detailedLogging = newDetailedLogging;
            // If monitoring is already enabled, update the setting
            if (EditorApplication.isPlaying)
            {
                RxPerformanceMonitor.Enable(detailedLogging);
            }
        }

        double newSlowThreshold = EditorGUILayout.DoubleField("Slow Notification Threshold (ms)", slowThreshold);
        if (Math.Abs(newSlowThreshold - slowThreshold) > 0.001)
        {
            slowThreshold = newSlowThreshold;
            // If monitoring is already enabled, update the setting
            if (EditorApplication.isPlaying)
            {
                RxPerformanceMonitor.SetSlowNotificationThreshold(slowThreshold);
            }
        }

        autoRefresh = EditorGUILayout.Toggle("Auto-Refresh Report", autoRefresh);

        EditorGUILayout.Space();

        // Controls
        EditorGUILayout.BeginHorizontal();

        GUI.enabled = EditorApplication.isPlaying;

        if (GUILayout.Button("Enable Monitoring", GUILayout.Height(30)))
        {
            RxPerformanceMonitor.Enable(detailedLogging);
            RxPerformanceMonitor.SetSlowNotificationThreshold(slowThreshold);
            RefreshReport();
        }

        if (GUILayout.Button("Disable Monitoring", GUILayout.Height(30)))
        {
            RxPerformanceMonitor.Disable();
            reportText = "Monitoring disabled. Enable monitoring to see performance metrics.";
        }

        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Reset Metrics", GUILayout.Height(30)))
        {
            RxPerformanceMonitor.Reset();
            RefreshReport();
        }

        if (GUILayout.Button("Refresh Report", GUILayout.Height(30)))
        {
            RefreshReport();
        }

        GUI.enabled = true;

        EditorGUILayout.Space();

        // Report
        EditorGUILayout.LabelField("Performance Report", EditorStyles.boldLabel);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(400));
        EditorGUILayout.TextArea(reportText, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();
    }

    private void RefreshReport()
    {
        if (!EditorApplication.isPlaying)
        {
            reportText = "Enter play mode to enable performance monitoring";
            return;
        }

        reportText = RxPerformanceMonitor.GenerateReport();
    }
}
#endif