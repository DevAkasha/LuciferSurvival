using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Utility class to inject tracking code into Rx objects 
/// This uses reflection to modify existing objects without changing base classes
/// </summary>
public static class RxTrackableInjector
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD || RXDEBUG
    private static readonly List<WeakReference<RxBase>> injectedRxObjects = new();
    private static readonly HashSet<Type> scannedTypes = new();
    private static readonly Dictionary<RxBase, List<Delegate>> originalMethods = new();

    private static bool isInitialized = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        // Check if tracking is enabled via preprocessor define
        if (IsTrackingEnabled())
        {
            Application.quitting += OnApplicationQuitting;
            isInitialized = true;
            Debug.Log("[RxTrackableInjector] Initialized");
        }
    }

    private static bool IsTrackingEnabled()
    {
        // Check for the RXDEBUG define
#if RXDEBUG
        return true;
#else
        return false;
#endif
    }

    private static void OnApplicationQuitting()
    {
        // Return all original methods to avoid issues on assembly reload
        RestoreAllOriginalMethods();

        // Perform final memory leak check
        CheckInjectedObjectsForLeaks();
    }

    /// <summary>
    /// Inject tracking code into RxVar AddListener/RemoveListener methods
    /// </summary>
    public static void InjectTrackingIntoRxVar<T>(RxVar<T> rxVar, string context = null)
    {
        if (!isInitialized) return;

        try
        {
            Type rxVarType = typeof(RxVar<T>);

            // Get the original methods using reflection
            MethodInfo addListenerMethod = rxVarType.GetMethod("AddListener");
            MethodInfo removeListenerMethod = rxVarType.GetMethod("RemoveListener");

            // Store original delegates
            Action<Action<T>> originalAddListener = (Action<Action<T>>)Delegate.CreateDelegate(
                typeof(Action<Action<T>>), rxVar, addListenerMethod);

            Action<Action<T>> originalRemoveListener = (Action<Action<T>>)Delegate.CreateDelegate(
                typeof(Action<Action<T>>), rxVar, removeListenerMethod);

            if (!originalMethods.ContainsKey(rxVar))
            {
                originalMethods[rxVar] = new List<Delegate>();
            }

            originalMethods[rxVar].Add(originalAddListener);
            originalMethods[rxVar].Add(originalRemoveListener);

            // Create new methods with tracking
            Action<Action<T>> newAddListener = (listener) => {
                // Call original method
                originalAddListener(listener);

                if (listener != null)
                {
                    // Record the subscription
                    RxMemoryTracker.TrackSubscription(rxVar, listener.Target);
                    rxVar.RecordSubscription(listener.Target);
                }
            };

            Action<Action<T>> newRemoveListener = (listener) => {
                // Just call original method
                originalRemoveListener(listener);
            };

            // Use reflection to replace the original delegates with our tracking versions
            // Note: This is advanced and potentially dangerous - it modifies the instance's function pointers
            // You would need to implement method swapping appropriate for your platform and runtime

            // For demonstration only - actual replacement would require unsafe code or platform-specific techniques
            // ReplaceMethod(rxVar, "AddListener", newAddListener);
            // ReplaceMethod(rxVar, "RemoveListener", newRemoveListener);

            // Instead, let's track the object in the RxMemoryTracker
            RxMemoryTracker.TrackObject(rxVar, context ?? $"RxVar<{typeof(T).Name}>");

            // Add to our injected objects list for monitoring
            injectedRxObjects.Add(new WeakReference<RxBase>(rxVar));
        }
        catch (Exception ex)
        {
            Debug.LogError($"[RxTrackableInjector] Failed to inject tracking: {ex.Message}");
        }
    }

    /// <summary>
    /// Inject tracking code into FSM AddListener/RemoveListener methods
    /// </summary>
    public static void InjectTrackingIntoFSM<TState>(FSM<TState> fsm, string context = null) where TState : Enum
    {
        if (!isInitialized) return;

        try
        {
            // Add to our injected objects list for monitoring
            injectedRxObjects.Add(new WeakReference<RxBase>(fsm));

            // Track using RxMemoryTracker
            RxMemoryTracker.TrackObject(fsm, context ?? $"FSM<{typeof(TState).Name}>");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[RxTrackableInjector] Failed to inject tracking: {ex.Message}");
        }
    }

    /// <summary>
    /// Inject tracking code into a BaseModel and all its Rx fields
    /// </summary>
    public static void InjectTrackingIntoModel(BaseModel model, string context = null)
    {
        if (!isInitialized) return;

        try
        {
            // Track the model itself
            RxMemoryTracker.TrackObject(model, context ?? $"Model of type {model.GetType().Name}");

            // Find and track all RxBase fields
            foreach (var rxField in model.GetAllRxFields())
            {
                if (rxField == null) continue;

                injectedRxObjects.Add(new WeakReference<RxBase>(rxField));
                RxMemoryTracker.TrackObject(rxField, $"RxField in {model.GetType().Name}");

                // Try to inject tracking based on the field type
                if (rxField is RxVar<int> rxVarInt)
                {
                    InjectTrackingIntoRxVar(rxVarInt, $"RxVar<int> in {model.GetType().Name}");
                }
                else if (rxField is RxVar<float> rxVarFloat)
                {
                    InjectTrackingIntoRxVar(rxVarFloat, $"RxVar<float> in {model.GetType().Name}");
                }
                // Add more type checks as needed for your specific RxVar types
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[RxTrackableInjector] Failed to inject tracking into model: {ex.Message}");
        }
    }

    /// <summary>
    /// Inject tracking code into all Rx objects in a MonoBehaviour
    /// </summary>
    public static void InjectTrackingIntoMonoBehaviour(MonoBehaviour behaviour)
    {
        if (!isInitialized) return;

        try
        {
            Type type = behaviour.GetType();

            // Check if we've already scanned this type
            if (scannedTypes.Contains(type))
                return;

            scannedTypes.Add(type);

            // Get all fields of the MonoBehaviour
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                // Check if field is RxBase or a subclass
                if (typeof(RxBase).IsAssignableFrom(field.FieldType))
                {
                    var rxObj = field.GetValue(behaviour) as RxBase;
                    if (rxObj != null)
                    {
                        injectedRxObjects.Add(new WeakReference<RxBase>(rxObj));
                        RxMemoryTracker.TrackObject(rxObj, $"{field.Name} in {type.Name}");

                        // Track specific Rx types
                        if (rxObj is FSM<Enum> fsm)
                        {
                            InjectTrackingIntoFSM(fsm, $"{field.Name} in {type.Name}");
                        }
                        else if (rxObj.GetType().IsGenericType && rxObj.GetType().GetGenericTypeDefinition() == typeof(RxVar<>))
                        {
                            // We need to use reflection to call the correct InjectTrackingIntoRxVar<T> method
                            Type genericType = rxObj.GetType().GetGenericArguments()[0];
                            MethodInfo injectMethod = typeof(RxTrackableInjector).GetMethod("InjectTrackingIntoRxVar")
                                .MakeGenericMethod(genericType);

                            injectMethod.Invoke(null, new object[] { rxObj, $"{field.Name} in {type.Name}" });
                        }
                    }
                }
                else if (typeof(BaseModel).IsAssignableFrom(field.FieldType))
                {
                    var model = field.GetValue(behaviour) as BaseModel;
                    if (model != null)
                    {
                        InjectTrackingIntoModel(model, $"{field.Name} in {type.Name}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[RxTrackableInjector] Failed to inject tracking into MonoBehaviour: {ex.Message}");
        }
    }

    /// <summary>
    /// Restore original method implementations to avoid issues during assembly reloading
    /// </summary>
    private static void RestoreAllOriginalMethods()
    {
        foreach (var entry in originalMethods)
        {
            // In a real implementation, this would restore the original method pointers
            // Again, this would require unsafe code or platform-specific techniques
        }

        originalMethods.Clear();
    }

    /// <summary>
    /// Check if any of our injected objects leaked
    /// </summary>
    private static void CheckInjectedObjectsForLeaks()
    {
        int leakCount = 0;

        // Force garbage collection before checking
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Check which objects are still alive
        for (int i = injectedRxObjects.Count - 1; i >= 0; i--)
        {
            var weakRef = injectedRxObjects[i];

            if (weakRef.TryGetTarget(out var target))
            {
                leakCount++;
                Debug.LogWarning($"[RxTrackableInjector] Potential memory leak: {target.GetType().Name} is still alive");
            }
            else
            {
                // Object was properly collected
                injectedRxObjects.RemoveAt(i);
            }
        }

        if (leakCount > 0)
        {
            Debug.LogWarning($"[RxTrackableInjector] Found {leakCount} potential Rx object leaks");
        }
        else
        {
            Debug.Log("[RxTrackableInjector] No Rx object leaks detected");
        }
    }

    /// <summary>
    /// Run a memory leak check on all tracked objects
    /// </summary>
    public static void RunMemoryLeakCheck()
    {
        if (!isInitialized) return;

        CheckInjectedObjectsForLeaks();
        RxMemoryTracker.DumpLeaks();
        RxMemoryTracker.DetectCyclicSubscriptions();
    }
#else
    // Stub implementations for release builds - compiler will optimize these out
    public static void InjectTrackingIntoRxVar<T>(RxVar<T> rxVar, string context = null) { }
    public static void InjectTrackingIntoFSM<TState>(FSM<TState> fsm, string context = null) where TState : Enum { }
    public static void InjectTrackingIntoModel(BaseModel model, string context = null) { }
    public static void InjectTrackingIntoMonoBehaviour(MonoBehaviour behaviour) { }
    public static void RunMemoryLeakCheck() { }
#endif
}

#if UNITY_EDITOR
/// <summary>
/// Editor window for RxTrackableInjector control
/// </summary>
[InitializeOnLoad]
public class RxTrackableInjectorWindow : EditorWindow
{
    private bool autoTrackNewObjects = false;
    private bool injectOnAwake = false;
    private bool injectOnSceneChange = false;

    private Vector2 scrollPosition;

    static RxTrackableInjectorWindow()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            // Auto-enable tracking when entering play mode
            if (EditorPrefs.GetBool("RxTrackableInjector.AutoTrack", false))
            {
                RxMemoryTracker.Enable(EditorPrefs.GetBool("RxTrackableInjector.AutoTrackNew", false));
            }
        }
    }

    [MenuItem("Tools/Rx Framework/Trackable Injector")]
    public static void ShowWindow()
    {
        GetWindow<RxTrackableInjectorWindow>("Rx Injector");
    }

    private void OnEnable()
    {
        autoTrackNewObjects = EditorPrefs.GetBool("RxTrackableInjector.AutoTrackNew", false);
        injectOnAwake = EditorPrefs.GetBool("RxTrackableInjector.InjectOnAwake", false);
        injectOnSceneChange = EditorPrefs.GetBool("RxTrackableInjector.InjectOnSceneChange", false);
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.LabelField("Rx Trackable Injector", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Injects memory tracking code into Rx objects at runtime", MessageType.Info);

        EditorGUILayout.Space();

        bool autoTrack = EditorGUILayout.Toggle("Auto-Enable Tracking on Play",
            EditorPrefs.GetBool("RxTrackableInjector.AutoTrack", false));
        EditorPrefs.SetBool("RxTrackableInjector.AutoTrack", autoTrack);

        autoTrackNewObjects = EditorGUILayout.Toggle("Auto-Track New Objects", autoTrackNewObjects);
        EditorPrefs.SetBool("RxTrackableInjector.AutoTrackNew", autoTrackNewObjects);

        injectOnAwake = EditorGUILayout.Toggle("Inject Tracking on Awake", injectOnAwake);
        EditorPrefs.SetBool("RxTrackableInjector.InjectOnAwake", injectOnAwake);

        injectOnSceneChange = EditorGUILayout.Toggle("Inject on Scene Change", injectOnSceneChange);
        EditorPrefs.SetBool("RxTrackableInjector.InjectOnSceneChange", injectOnSceneChange);

        EditorGUILayout.Space();

        if (GUILayout.Button("Inject Tracking into Current Scene", GUILayout.Height(30)))
        {
            InjectTrackingIntoCurrentScene();
        }

        if (GUILayout.Button("Run Memory Leak Check", GUILayout.Height(30)))
        {
            RxTrackableInjector.RunMemoryLeakCheck();
        }

        EditorGUILayout.EndVertical();
    }

    private void InjectTrackingIntoCurrentScene()
    {
        if (!Application.isPlaying)
        {
            EditorUtility.DisplayDialog("Error", "Injection only works in Play mode", "OK");
            return;
        }

        var behaviours = FindObjectsOfType<MonoBehaviour>();
        int count = 0;

        foreach (var behaviour in behaviours)
        {
            RxTrackableInjector.InjectTrackingIntoMonoBehaviour(behaviour);
            count++;
        }

        EditorUtility.DisplayDialog("Injection Complete",
            $"Injected tracking into {count} MonoBehaviours", "OK");
    }
}
#endif