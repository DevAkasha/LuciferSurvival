using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Detects and reports circular dependencies in the Rx framework.
/// Circular dependencies can cause infinite loops when values change.
/// </summary>
public static class RxCyclicDetector
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD || RXDEBUG
    private static readonly Dictionary<Type, Dictionary<RxBase, HashSet<RxBase>>> subscriptionGraph =
        new Dictionary<Type, Dictionary<RxBase, HashSet<RxBase>>>();

    private static readonly HashSet<(RxBase, RxBase)> detectedCycles = new HashSet<(RxBase, RxBase)>();

    private static bool isEnabled = false;

    /// <summary>
    /// Enable cyclic dependency detection
    /// </summary>
    public static void Enable()
    {
        isEnabled = true;
        Debug.Log("[RxCyclicDetector] Enabled cycle detection");
    }

    /// <summary>
    /// Disable cyclic dependency detection
    /// </summary>
    public static void Disable()
    {
        isEnabled = false;
        subscriptionGraph.Clear();
        detectedCycles.Clear();
        Debug.Log("[RxCyclicDetector] Disabled cycle detection");
    }

    /// <summary>
    /// Record a subscription from publisher to subscriber
    /// </summary>
    public static void RecordSubscription(RxBase publisher, RxBase subscriber)
    {
        if (!isEnabled || publisher == null || subscriber == null) return;

        Type publisherType = publisher.GetType();

        if (!subscriptionGraph.TryGetValue(publisherType, out var typeSubscriptions))
        {
            typeSubscriptions = new Dictionary<RxBase, HashSet<RxBase>>();
            subscriptionGraph[publisherType] = typeSubscriptions;
        }

        if (!typeSubscriptions.TryGetValue(publisher, out var subscriptions))
        {
            subscriptions = new HashSet<RxBase>();
            typeSubscriptions[publisher] = subscriptions;
        }

        subscriptions.Add(subscriber);

        // Check for cycles when adding a new subscription
        CheckForCycles(publisher, subscriber);
    }

    /// <summary>
    /// Remove a subscription (used when listeners are removed)
    /// </summary>
    public static void RemoveSubscription(RxBase publisher, RxBase subscriber)
    {
        if (!isEnabled || publisher == null || subscriber == null) return;

        Type publisherType = publisher.GetType();

        if (subscriptionGraph.TryGetValue(publisherType, out var typeSubscriptions))
        {
            if (typeSubscriptions.TryGetValue(publisher, out var subscriptions))
            {
                subscriptions.Remove(subscriber);

                // Remove empty subscriptions to prevent memory leaks
                if (subscriptions.Count == 0)
                {
                    typeSubscriptions.Remove(publisher);
                }
            }

            // Remove empty type dictionary
            if (typeSubscriptions.Count == 0)
            {
                subscriptionGraph.Remove(publisherType);
            }
        }

        // Clean up any detected cycles involving this pair
        detectedCycles.RemoveWhere(pair =>
            (pair.Item1 == publisher && pair.Item2 == subscriber) ||
            (pair.Item1 == subscriber && pair.Item2 == publisher));
    }

    /// <summary>
    /// Check the entire graph for cyclic dependencies
    /// </summary>
    public static bool DetectAllCycles()
    {
        if (!isEnabled) return false;

        detectedCycles.Clear();
        bool found = false;

        foreach (var typePair in subscriptionGraph)
        {
            foreach (var pubSub in typePair.Value)
            {
                HashSet<RxBase> visited = new HashSet<RxBase>();
                if (DetectCycleDFS(pubSub.Key, pubSub.Key, visited, true))
                {
                    found = true;
                }
            }
        }

        if (found)
        {
            ReportCycles();
        }

        return found;
    }

    /// <summary>
    /// Check for cycles between publisher and a new subscriber
    /// </summary>
    private static void CheckForCycles(RxBase publisher, RxBase subscriber)
    {
        // Skip self-cycles (already handled elsewhere)
        if (publisher == subscriber) return;

        // Check if subscriber is already a publisher that eventually targets the original publisher
        HashSet<RxBase> visited = new HashSet<RxBase>();
        if (DetectCycleDFS(subscriber, publisher, visited, false))
        {
            // Record this cycle
            detectedCycles.Add((publisher, subscriber));

            // Report immediately
            Debug.LogWarning($"[RxCyclicDetector] Detected a cyclic dependency between {publisher.GetType().Name} and {subscriber.GetType().Name}");
        }
    }

    /// <summary>
    /// Depth-first search for cyclic dependencies
    /// </summary>
    private static bool DetectCycleDFS(RxBase current, RxBase target, HashSet<RxBase> visited, bool reportAll)
    {
        if (visited.Contains(current)) return false;
        visited.Add(current);

        Type currentType = current.GetType();

        if (subscriptionGraph.TryGetValue(currentType, out var typeSubscriptions))
        {
            if (typeSubscriptions.TryGetValue(current, out var subscriptions))
            {
                foreach (var subscriber in subscriptions)
                {
                    if (subscriber == target)
                    {
                        if (reportAll)
                        {
                            detectedCycles.Add((current, subscriber));
                        }
                        return true;
                    }

                    // Continue the search
                    if (DetectCycleDFS(subscriber, target, new HashSet<RxBase>(visited), reportAll))
                    {
                        if (reportAll)
                        {
                            detectedCycles.Add((current, subscriber));
                        }
                        return true;
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Report all detected cycles to the console
    /// </summary>
    private static void ReportCycles()
    {
        if (detectedCycles.Count == 0) return;

        StringBuilder report = new StringBuilder();
        report.AppendLine($"[RxCyclicDetector] Found {detectedCycles.Count} cyclic dependencies:");

        foreach (var cycle in detectedCycles)
        {
            report.AppendLine($"  - Cycle between {cycle.Item1.GetType().Name} and {cycle.Item2.GetType().Name}");
        }

        Debug.LogWarning(report.ToString());
    }

    /// <summary>
    /// Get a list of cyclic dependencies for debugging
    /// </summary>
    public static List<(RxBase, RxBase)> GetDetectedCycles()
    {
        return detectedCycles.ToList();
    }

    /// <summary>
    /// Check if an object is involved in any cyclic dependencies
    /// </summary>
    public static bool IsInCycle(RxBase rxObject)
    {
        if (!isEnabled || rxObject == null) return false;

        foreach (var cycle in detectedCycles)
        {
            if (cycle.Item1 == rxObject || cycle.Item2 == rxObject)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Get the subscription graph size for a specific type
    /// </summary>
    public static int GetTypeSubscriptionCount(Type type)
    {
        if (!isEnabled || !subscriptionGraph.TryGetValue(type, out var typeSubscriptions))
        {
            return 0;
        }

        return typeSubscriptions.Count;
    }

    /// <summary>
    /// Get information about a specific object's subscriptions
    /// </summary>
    public static (int publishers, int subscribers) GetObjectSubscriptionCounts(RxBase rxObject)
    {
        if (!isEnabled || rxObject == null) return (0, 0);

        int publisherCount = 0;
        int subscriberCount = 0;

        Type objectType = rxObject.GetType();

        // Count subscribers
        if (subscriptionGraph.TryGetValue(objectType, out var typeSubscriptions))
        {
            if (typeSubscriptions.TryGetValue(rxObject, out var subscriptions))
            {
                subscriberCount = subscriptions.Count;
            }
        }

        // Count publishers (this is more expensive)
        foreach (var typePair in subscriptionGraph)
        {
            foreach (var pubSub in typePair.Value)
            {
                if (pubSub.Value.Contains(rxObject))
                {
                    publisherCount++;
                }
            }
        }

        return (publisherCount, subscriberCount);
    }
#else
    // Stub implementations for release builds
    public static void Enable() { }
    public static void Disable() { }
    public static void RecordSubscription(RxBase publisher, RxBase subscriber) { }
    public static void RemoveSubscription(RxBase publisher, RxBase subscriber) { }
    public static bool DetectAllCycles() { return false; }
    public static List<(RxBase, RxBase)> GetDetectedCycles() { return new List<(RxBase, RxBase)>(); }
    public static bool IsInCycle(RxBase rxObject) { return false; }
    public static int GetTypeSubscriptionCount(Type type) { return 0; }
    public static (int publishers, int subscribers) GetObjectSubscriptionCounts(RxBase rxObject) { return (0, 0); }
#endif
}


/// <summary>
/// Editor window for cycle detection visualization and management
/// </summary>
public class RxCyclicDetectorWindow : EditorWindow
{
    private Vector2 scrollPosition;
    private bool autoDetectOnCompile = false;
    private Dictionary<RxBase, GameObject> rxBaseToGameObjectMap = new Dictionary<RxBase, GameObject>();

    [MenuItem("Tools/Rx Framework/Cyclic Detector")]
    public static void ShowWindow()
    {
        GetWindow<RxCyclicDetectorWindow>("Rx Cycle Detector");
    }

    private void OnEnable()
    {
        autoDetectOnCompile = EditorPrefs.GetBool("RxCyclicDetector.AutoDetect", false);
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.LabelField("Rx Cyclic Dependency Detector", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Detects circular references that could cause infinite notification loops", MessageType.Info);

        EditorGUILayout.Space();

        bool autoDetect = EditorGUILayout.Toggle("Auto-Detect on Compile", autoDetectOnCompile);
        if (autoDetect != autoDetectOnCompile)
        {
            autoDetectOnCompile = autoDetect;
            EditorPrefs.SetBool("RxCyclicDetector.AutoDetect", autoDetectOnCompile);
        }

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Enable Detection", GUILayout.Height(30)))
        {
            RxCyclicDetector.Enable();
        }

        if (GUILayout.Button("Disable Detection", GUILayout.Height(30)))
        {
            RxCyclicDetector.Disable();
        }

        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Detect Cycles Now", GUILayout.Height(30)))
        {
            bool found = RxCyclicDetector.DetectAllCycles();
            if (found)
            {
                // 사이클이 감지되었을 때 RxBase -> GameObject 맵핑 갱신
                BuildRxBaseToGameObjectMap();
            }
            else
            {
                EditorUtility.DisplayDialog("No Cycles Found", "No circular dependencies were detected.", "OK");
            }
        }

        EditorGUILayout.Space();

        // Display detected cycles
        var cycles = RxCyclicDetector.GetDetectedCycles();
        if (cycles.Count > 0)
        {
            EditorGUILayout.LabelField($"Detected {cycles.Count} Cycles:", EditorStyles.boldLabel);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));

            foreach (var cycle in cycles)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                // 객체 정보 표시
                string item1Type = cycle.Item1.GetType().Name;
                string item2Type = cycle.Item2.GetType().Name;

                EditorGUILayout.LabelField($"Cycle between {item1Type} and {item2Type}", EditorStyles.boldLabel);

                // 첫 번째 객체 정보 및 선택 버튼
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"1: {item1Type}", GUILayout.Width(150));

                GameObject go1 = FindGameObjectFor(cycle.Item1);
                if (go1 != null)
                {
                    EditorGUILayout.LabelField($"GameObject: {go1.name}");
                    if (GUILayout.Button("Select", GUILayout.Width(60)))
                    {
                        Selection.activeGameObject = go1;
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("Not in Scene");
                }
                EditorGUILayout.EndHorizontal();

                // 두 번째 객체 정보 및 선택 버튼
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"2: {item2Type}", GUILayout.Width(150));

                GameObject go2 = FindGameObjectFor(cycle.Item2);
                if (go2 != null)
                {
                    EditorGUILayout.LabelField($"GameObject: {go2.name}");
                    if (GUILayout.Button("Select", GUILayout.Width(60)))
                    {
                        Selection.activeGameObject = go2;
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("Not in Scene");
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.EndVertical();
    }

    private void BuildRxBaseToGameObjectMap()
    {
        // 맵 초기화
        rxBaseToGameObjectMap.Clear();

        // 모든 MonoBehaviour 찾기
        var allMonoBehaviours = Resources.FindObjectsOfTypeAll<MonoBehaviour>();

        foreach (var mb in allMonoBehaviours)
        {
            // null이거나 이미 파괴된 객체 또는 prefab 인스턴스는 건너뛰기
            if (mb == null || mb.gameObject == null || !mb.gameObject.scene.IsValid())
                continue;

            // 모든 public/private 필드 검사
            var allFields = mb.GetType().GetFields(
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic);

            foreach (var field in allFields)
            {
                // RxBase 타입이거나 상속한 필드 검색
                if (typeof(RxBase).IsAssignableFrom(field.FieldType))
                {
                    try
                    {
                        var value = field.GetValue(mb) as RxBase;
                        if (value != null && !rxBaseToGameObjectMap.ContainsKey(value))
                        {
                            rxBaseToGameObjectMap[value] = mb.gameObject;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error accessing field {field.Name} on {mb.name}: {ex.Message}");
                    }
                }
            }
        }

        // 디버그용: 맵에 몇 개의 항목이 추가되었는지 로그
        Debug.Log($"[RxCyclicDetector] Built mapping for {rxBaseToGameObjectMap.Count} RxBase objects to GameObjects");
    }

    private GameObject FindGameObjectFor(RxBase rxObject)
    {
        if (rxObject == null)
            return null;

        // 맵이 비어 있으면 다시 구축
        if (rxBaseToGameObjectMap.Count == 0)
            BuildRxBaseToGameObjectMap();

        // 맵에서 GameObject 찾기
        if (rxBaseToGameObjectMap.TryGetValue(rxObject, out var go))
            return go;

        return null;
    }
}