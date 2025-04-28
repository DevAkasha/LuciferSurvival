using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Central debugging utility for the Rx framework
/// Provides monitoring and visualization of Rx objects
/// </summary>
public static class RxDebugger
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD || RXDEBUG
    private static bool isEnabled = true;
    private static bool showPerformanceMetrics = true;
    private static bool showMemoryUsage = true;
    private static bool highlightCyclicDependencies = true;

    private static readonly List<NotificationEvent> recentNotifications = new List<NotificationEvent>();

    private class NotificationEvent
    {
        public RxBase Source;
        public DateTime Time;
        public float Duration;
        public int ListenerCount;
    }

    /// <summary>
    /// Enable the Rx debugger
    /// </summary>
    public static void Enable()
    {
        isEnabled = true;

        // Also enable related systems
        RxMemoryTracker.Enable(true);
        RxCyclicDetector.Enable();
        RxPerformanceMonitor.Enable(false);

        Debug.Log("[RxDebugger] Enabled");
    }

    /// <summary>
    /// Disable the Rx debugger
    /// </summary>
    public static void Disable()
    {
        isEnabled = false;

        // Also disable related systems
        RxCyclicDetector.Disable();
        RxPerformanceMonitor.Disable();

        Debug.Log("[RxDebugger] Disabled");
    }

    /// <summary>
    /// Configure debugger settings
    /// </summary>
    public static void Configure(bool performance = true, bool memory = true, bool cyclic = true)
    {
        showPerformanceMetrics = performance;
        showMemoryUsage = memory;
        highlightCyclicDependencies = cyclic;

        // Update detailed logging in performance monitor
        if (isEnabled && showPerformanceMetrics)
        {
            RxPerformanceMonitor.Enable(true);
        }
    }

    /// <summary>
    /// Record notification performance
    /// </summary>
    public static void RecordNotification(RxBase source, float durationMs, int listenerCount)
    {
        if (!isEnabled || !showPerformanceMetrics) return;

        // Record in performance monitor
        RxPerformanceMonitor.EndMeasurement(source, listenerCount);

        // Record event for local tracking
        recentNotifications.Add(new NotificationEvent
        {
            Source = source,
            Time = DateTime.Now,
            Duration = durationMs,
            ListenerCount = listenerCount
        });

        // Keep only recent events
        if (recentNotifications.Count > 100)
        {
            recentNotifications.RemoveAt(0);
        }

        // Log slow notifications
        if (durationMs > 5.0f) // Adjust threshold as needed
        {
            Debug.LogWarning($"[RxDebugger] Slow notification: {source.GetType().Name} took {durationMs:F2}ms with {listenerCount} listeners");
        }
    }

    /// <summary>
    /// Get performance report
    /// </summary>
    public static string GetPerformanceReport()
    {
        if (!isEnabled)
            return "Debugging is disabled";

        return RxPerformanceMonitor.GenerateReport();
    }

    /// <summary>
    /// Run a full diagnosis check of the Rx system
    /// </summary>
    public static void RunFullDiagnosis()
    {
        if (!isEnabled)
        {
            Debug.Log("[RxDebugger] Debugging is disabled. Enable it first with RxDebugger.Enable()");
            return;
        }

        // Performance check
        string perfReport = GetPerformanceReport();
        Debug.Log(perfReport);

        // Memory check
        RxMemoryTracker.DumpLeaks();

        // Cyclic dependency check
        RxCyclicDetector.DetectAllCycles();
    }

    /// <summary>
    /// Clear collected data
    /// </summary>
    public static void ClearData()
    {
        recentNotifications.Clear();
        RxPerformanceMonitor.Reset();
        Debug.Log("[RxDebugger] Data cleared");
    }

    /// <summary>
    /// Get diagnostic information for a specific RxBase object
    /// </summary>
    public static string GetObjectDiagnostics(RxBase obj)
    {
        if (!isEnabled || obj == null)
            return "Debugging is disabled or null object";

        StringBuilder info = new StringBuilder();
        info.AppendLine($"=== Diagnostics for {obj.GetType().Name} ===");

        // Performance data
        info.AppendLine(RxPerformanceMonitor.GetObjectMetrics(obj));

        // Cyclic check
        bool isInCycle = RxCyclicDetector.IsInCycle(obj);
        info.AppendLine($"In cyclic dependency: {isInCycle}");

        // Subscription counts
        var (publishers, subscribers) = RxCyclicDetector.GetObjectSubscriptionCounts(obj);
        info.AppendLine($"Publishers: {publishers}");
        info.AppendLine($"Subscribers: {subscribers}");

        // Object-specific data
        if (obj is IRxModFormulaProvider formulaProvider)
        {
            info.AppendLine($"Formula: {formulaProvider.BuildDebugFormula()}");
        }

        return info.ToString();
    }

    /// <summary>
    /// Prepare for notification measurement
    /// Call this before dispatching notifications
    /// </summary>
    public static void BeginNotification()
    {
        if (!isEnabled || !showPerformanceMetrics) return;
        RxPerformanceMonitor.StartMeasurement();
    }
#else
    // Stub implementations for release builds
    public static void Enable() { }
    public static void Disable() { }
    public static void Configure(bool performance = true, bool memory = true, bool cyclic = true) { }
    public static void RecordNotification(RxBase source, float durationMs, int listenerCount) { }
    public static string GetPerformanceReport() { return string.Empty; }
    public static void RunFullDiagnosis() { }
    public static void ClearData() { }
    public static string GetObjectDiagnostics(RxBase obj) { return string.Empty; }
    public static void BeginNotification() { }
#endif
}

#if UNITY_EDITOR

/// <summary>
/// Editor window for the Rx Debugger
/// </summary>
public class RxDebuggerWindow : EditorWindow
{
    private bool showPerformance = true;
    private bool showMemory = true;
    private bool showCycles = true;
    private Vector2 scrollPosition;
    private string selectedTab = "Performance";
    private double refreshInterval = 2.0; // seconds
    private double lastRefreshTime = 0.0;
    private bool autoRefresh = true;

    // RxBase 객체와 GameObject 간의 매핑
    private Dictionary<RxBase, GameObject> rxBaseToGameObjectMap = new Dictionary<RxBase, GameObject>();

    [MenuItem("Tools/Rx Framework/Rx Debugger")]
    public static void ShowWindow()
    {
        GetWindow<RxDebuggerWindow>("Rx Debugger");
    }

    private void OnEnable()
    {
        // Load saved preferences
        autoRefresh = EditorPrefs.GetBool("RxDebugger.AutoRefresh", true);
    }

    private void OnDisable()
    {
        // Save preferences
        EditorPrefs.SetBool("RxDebugger.AutoRefresh", autoRefresh);
    }

    private void Update()
    {
        if (autoRefresh && EditorApplication.isPlaying)
        {
            double currentTime = EditorApplication.timeSinceStartup;
            if (currentTime - lastRefreshTime > refreshInterval)
            {
                Repaint();
                lastRefreshTime = currentTime;
            }
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Rx Framework Debugger", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        // Main actions
        EditorGUILayout.BeginHorizontal();
        GUI.enabled = EditorApplication.isPlaying;

        if (GUILayout.Button("Enable", GUILayout.Height(30)))
        {
            RxDebugger.Enable();
        }

        if (GUILayout.Button("Disable", GUILayout.Height(30)))
        {
            RxDebugger.Disable();
        }

        if (GUILayout.Button("Clear Data", GUILayout.Height(30)))
        {
            RxDebugger.ClearData();
        }

        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // Options
        EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
        showPerformance = EditorGUILayout.Toggle("Show Performance Metrics", showPerformance);
        showMemory = EditorGUILayout.Toggle("Show Memory Usage", showMemory);
        showCycles = EditorGUILayout.Toggle("Highlight Cyclic Dependencies", showCycles);
        autoRefresh = EditorGUILayout.Toggle("Auto-Refresh", autoRefresh);

        if (GUILayout.Button("Apply Configuration"))
        {
            RxDebugger.Configure(showPerformance, showMemory, showCycles);
        }

        EditorGUILayout.Space();

        // Tabs
        EditorGUILayout.BeginHorizontal();
        GUI.enabled = EditorApplication.isPlaying;

        if (GUILayout.Toggle(selectedTab == "Performance", "Performance", EditorStyles.toolbarButton))
            selectedTab = "Performance";

        if (GUILayout.Toggle(selectedTab == "Memory", "Memory", EditorStyles.toolbarButton))
            selectedTab = "Memory";

        if (GUILayout.Toggle(selectedTab == "Cycles", "Cycles", EditorStyles.toolbarButton))
            selectedTab = "Cycles";

        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // Tab content
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(400));

        if (selectedTab == "Performance")
        {
            ShowPerformanceTab();
        }
        else if (selectedTab == "Memory")
        {
            ShowMemoryTab();
        }
        else if (selectedTab == "Cycles")
        {
            ShowCyclesTab();
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();
    }

    private void ShowPerformanceTab()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorGUILayout.HelpBox("Enter play mode to see performance metrics", MessageType.Info);
            return;
        }

        EditorGUILayout.TextArea(RxDebugger.GetPerformanceReport(), GUILayout.ExpandHeight(true));
    }

    private void ShowMemoryTab()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorGUILayout.HelpBox("Enter play mode to see memory metrics", MessageType.Info);
            return;
        }

        if (GUILayout.Button("Run Memory Leak Check"))
        {
            RxMemoryTracker.DumpLeaks();
        }

        EditorGUILayout.HelpBox("Check the console for memory leak reports", MessageType.Info);
    }

    private void ShowCyclesTab()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorGUILayout.HelpBox("Enter play mode to detect cycles", MessageType.Info);
            return;
        }

        if (GUILayout.Button("Detect Cycles Now"))
        {
            bool found = RxCyclicDetector.DetectAllCycles();
            if (found)
            {
                // 사이클이 감지되었을 때 RxBase -> GameObject 매핑 갱신
                BuildRxBaseToGameObjectMap();
            }
            else
            {
                EditorUtility.DisplayDialog("No Cycles Found", "No circular dependencies were detected.", "OK");
            }
        }

        // Display detected cycles
        var cycles = RxCyclicDetector.GetDetectedCycles();
        if (cycles.Count > 0)
        {
            EditorGUILayout.LabelField($"Detected {cycles.Count} Cycles:", EditorStyles.boldLabel);

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
        }
        else
        {
            EditorGUILayout.HelpBox("No cyclic dependencies detected", MessageType.Info);
        }
    }

    // RxBase -> GameObject 매핑 구축
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
        Debug.Log($"[RxDebugger] Built mapping for {rxBaseToGameObjectMap.Count} RxBase objects to GameObjects");
    }

    // RxBase 객체에 대한 GameObject 찾기
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
#endif