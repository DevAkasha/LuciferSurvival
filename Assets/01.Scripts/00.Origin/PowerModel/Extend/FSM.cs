using System;
using System.Collections.Generic;

public class FSM<TState> : RxBase where TState : Enum
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
    }

    public RxVar<TState> State => state;
    public TState Value => state.Value;

    // 상태 변경 직접 요청
    public FSM<TState> Request(TState next)
    {
        if (!CanTransitTo(next)) return this;

        onExit.TryGetValue(Value, out var exit); exit?.Invoke();
        state.SetValue(next);
        onEnter.TryGetValue(next, out var enter); enter?.Invoke();

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
        listeners.Add(listener);
        listener(Value);
        return this;
    }

    public FSM<TState> RemoveListener(Action<TState> listener)
    {
        listeners.Remove(listener);
        return this;
    }

    private void NotifyAll(TState v)
    {
        foreach (var l in listeners)
            l(v);
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
}