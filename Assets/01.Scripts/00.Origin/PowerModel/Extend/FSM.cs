using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public partial class FSM<TState> : RxBase where TState : Enum
{
    private readonly RxVar<TState> state;
    private readonly Dictionary<TState, Func<TState, bool>> guards = new();
    private readonly Dictionary<TState, Action> onEnter = new();
    private readonly Dictionary<TState, Action> onExit = new();
    private readonly List<Action<TState>> listeners = new();
    private readonly Dictionary<TState, int> priorities = new();

    public FSM(TState initial, ITrackableRxModel owner = null)
    {
        state = new RxVar<TState>(initial, owner);
        state.AddListener(NotifyAll);

        if (owner != null)
            owner.RegisterRx(this);

#if UNITY_EDITOR || DEVELOPMENT_BUILD || RXDEBUG
        // Auto-register with memory tracker
        RxMemoryTracker.TrackObject(this, $"FSM<{typeof(TState).Name}> owned by {owner?.GetType().Name ?? "unknown"}");
#endif
    }

    public RxVar<TState> State => state;
    public TState Value => state.Value;

    // 상태 변경 직접 요청
    public FSM<TState> Request(TState next)
    {
        if (!CanTransitTo(next)) return this;

#if UNITY_EDITOR || DEVELOPMENT_BUILD || RXDEBUG
        Stopwatch sw = new Stopwatch();
        sw.Start();
#endif

        onExit.TryGetValue(Value, out var exit); exit?.Invoke();
        state.SetValue(next);
        onEnter.TryGetValue(next, out var enter); enter?.Invoke();

#if UNITY_EDITOR || DEVELOPMENT_BUILD || RXDEBUG
        sw.Stop();
        float elapsedMs = sw.ElapsedTicks / (float)TimeSpan.TicksPerMillisecond;

        // Record performance metrics
        RxDebugger.RecordNotification(this, elapsedMs, listeners.Count);

        // Log slow transitions
        if (elapsedMs > 5.0f) // Threshold for "slow" transitions
        {
            Debug.LogWarning($"[FSM] Slow transition to {next} took {elapsedMs:F2}ms");
        }
#endif

        return this;
    }

    public bool CanTransitTo(TState next)
    {
        if (EqualityComparer<TState>.Default.Equals(Value, next)) return false;
        return !guards.TryGetValue(next, out var cond) || cond(Value);
    }

    public FSM<TState> AddTransitionRule(TState to, Func<TState, bool> rule)
    {
        guards[to] = rule;
        return this;
    }

    public FSM<TState> OnEnter(TState state, Action callback)
    {
        if (!onEnter.ContainsKey(state)) onEnter[state] = callback;
        else onEnter[state] += callback;
        return this;
    }

    public FSM<TState> OnExit(TState state, Action callback)
    {
        if (!onExit.ContainsKey(state)) onExit[state] = callback;
        else onExit[state] += callback;
        return this;
    }

    public FSM<TState> AddListener(Action<TState> listener)
    {
        if (listener == null) return this;

#if UNITY_EDITOR || DEVELOPMENT_BUILD || RXDEBUG
        // Check for self-subscription which can cause infinite loops
        if (listener.Target == this)
        {
            Debug.LogWarning($"[FSM] Self-subscription detected in {this}! This may cause infinite loops.");
        }

        // Record the subscription for debugging
        this.RecordSubscription(listener.Target);
#endif

        listeners.Add(listener);
        listener(Value);
        return this;
    }

    public FSM<TState> RemoveListener(Action<TState> listener)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD || RXDEBUG
        // Remove subscription tracking
        if (listener != null)
        {
            this.RemoveSubscriptionRecord(listener.Target);
        }
#endif

        listeners.Remove(listener);
        return this;
    }

    private void NotifyAll(TState v)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD || RXDEBUG
        Stopwatch sw = new Stopwatch();
        sw.Start();
#endif

        // Cache the list count since listeners may change during notification
        int listenerCount = listeners.Count;

        // Take a snapshot to avoid modification issues
        var currentListeners = new Action<TState>[listenerCount];
        listeners.CopyTo(currentListeners);

        // Notify all listeners
        foreach (var l in currentListeners)
        {
            if (l != null) // Check in case it was removed during iteration
            {
                try
                {
                    l(v);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[FSM] Exception in listener: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD || RXDEBUG
        sw.Stop();
        float elapsedMs = sw.ElapsedTicks / (float)TimeSpan.TicksPerMillisecond;

        // Record performance metrics
        RxDebugger.RecordNotification(this, elapsedMs, listenerCount);
#endif
    }

    public override void ClearRelation()
    {
        listeners.Clear();
        state.ClearRelation();
    }

    // 우선순위 등록
    public FSM<TState> SetPriority(TState state, int priority)
    {
        priorities[state] = priority;
        return this;
    }

    // 우선순위 기반 전이 요청
    public void RequestByPriority(params TState[] candidates)
    {
        TState? best = default;
        int bestPriority = int.MinValue;

        foreach (var state in candidates)
        {
            if (!CanTransitTo(state)) continue;

            int p = priorities.GetValueOrDefault(state, 0);
            if (best == null || p > bestPriority || (p == bestPriority && Equals(state, Value)))
            {
                best = state;
                bestPriority = p;
            }
        }

        if (best != null && !Equals(best, Value))
            Request(best);
    }

    public void RequestByPriority(TState single)
    {
        RequestByPriority(new[] { single });
    }

    // 디버그 출력
    public FSM<TState> WithDebug(string tag = "[FSM]")
    {
        AddListener(state => UnityEngine.Debug.Log($"{tag} → {state}"));
        return this;
    }

    // RxFlagSet으로 상태 평가
    public FSM<TState> DriveByFlags<TFlag>(RxStateFlagSet<TFlag> flags, Func<RxStateFlagSet<TFlag>, TState> evaluator) where TFlag : Enum
    {
        EvaluateFlags(flags, evaluator);

        foreach (var (flag, _) in flags.Snapshot())
            flags.AddListener(flag, _ => EvaluateFlags(flags, evaluator));

        return this;
    }

    private void EvaluateFlags<TFlag>(RxStateFlagSet<TFlag> flags, Func<RxStateFlagSet<TFlag>, TState> evaluator) where TFlag : Enum
    {
        var next = evaluator(flags);
        RequestByPriority(next);
    }

    public override string ToString()
    {
        return $"FSM<{typeof(TState).Name}>({Value})";
    }
}

// FSM partial class implementation to add IRxInspectable interface
public partial class FSM<TState> : IRxInspectable where TState : Enum
{
#if UNITY_EDITOR
    public void DrawDebugInspector()
    {
        TState currentState = Value;

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        // FSM 타입 이름으로 헤더 표시
        EditorGUILayout.LabelField($"FSM<{typeof(TState).Name}>", EditorStyles.boldLabel);

        // 현재 상태를 녹색으로 강조 표시
        GUIStyle currentStateStyle = new(EditorStyles.boldLabel)
        {
            normal = { textColor = Color.green }
        };

        EditorGUILayout.LabelField("현재 상태:", currentState.ToString(), currentStateStyle);

        // 가능한 전이 섹션
        EditorGUILayout.Space(2);
        EditorGUILayout.LabelField("가능한 전이:", EditorStyles.boldLabel);

        // 모든 가능한 전이 나열
        foreach (TState state in Enum.GetValues(typeof(TState)))
        {
            if (EqualityComparer<TState>.Default.Equals(state, currentState))
                continue;

            bool canTransit = CanTransitTo(state);
            GUIStyle stateStyle = new(EditorStyles.label)
            {
                normal = { textColor = canTransit ? Color.cyan : Color.gray }
            };

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(state.ToString(), stateStyle, GUILayout.Width(150));

            GUI.enabled = canTransit;
            if (GUILayout.Button("전이", GUILayout.Width(60)))
            {
                Request(state);
            }
            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
    }
#endif
}
