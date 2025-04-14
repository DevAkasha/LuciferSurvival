using System;
using System.Collections.Generic;

public class FSM<TState> where TState : Enum
{
    private readonly RxStateMachine<TState> stateMachine;
    private readonly Dictionary<TState, int> priorities = new();

    public RxVar<TState> Current => stateMachine.Current;
    public TState Value => Current.Value;

    public FSM(TState initialState)
    {
        stateMachine = new RxStateMachine<TState>(initialState);
    }

    public FSM<TState> OnEnter(TState state, Action callback)
    {
        stateMachine.OnEnter(state, callback);
        return this;
    }

    public FSM<TState> OnExit(TState state, Action callback)
    {
        stateMachine.OnExit(state, callback);
        return this;
    }

    public FSM<TState> SetPriority(TState state, int priority)
    {
        priorities[state] = priority;
        return this;
    }

    public FSM<TState> WithFlagEvaluator<TFlag>(
        RxStateFlagSet<TFlag> flags,
        Func<RxStateFlagSet<TFlag>, TState> evaluator)
        where TFlag : Enum
    {
        EvaluateFlags(flags, evaluator);

        foreach (var (flag, _) in flags.Snapshot())
        {
            flags.AddListener(flag, _ => EvaluateFlags(flags, evaluator));
        }

        return this;
    }

    private void EvaluateFlags<TFlag>(
        RxStateFlagSet<TFlag> flags,
        Func<RxStateFlagSet<TFlag>, TState> evaluator)
        where TFlag : Enum
    {
        var next = evaluator(flags);
        Request(next);
    }

    public void Request(TState next)
    {
        stateMachine.Request(next);
    }

    public void RequestPriority(params TState[] candidates)
    {
        TState? best = default;
        int bestPriority = int.MinValue;

        foreach (var state in candidates)
        {
            if (!stateMachine.CanTransitTo(state)) continue;

            int p = priorities.GetValueOrDefault(state, 0);
            if (best == null || p > bestPriority)
            {
                best = state;
                bestPriority = p;
            }
        }

        if (best != null)
            Request(best);
    }
}
