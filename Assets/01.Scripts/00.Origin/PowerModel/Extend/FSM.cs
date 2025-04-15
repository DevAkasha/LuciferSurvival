using System;
using System.Collections.Generic;
using UnityEngine;

public class FSM<TState> where TState : Enum
{
    private readonly RxStateMachine<TState> stateMachine;
    private readonly Dictionary<TState, int> priorities = new();

    public RxVar<TState> State => stateMachine.Current;
    public TState Value => State.Value;

    public FSM(TState initialState)
    {
        stateMachine = new RxStateMachine<TState>(initialState);
    }

    // 상태 진입 시 콜백
    public FSM<TState> OnEnter(TState state, Action callback)
    {
        stateMachine.OnEnter(state, callback);
        return this;
    }

    // 상태 이탈 시 콜백
    public FSM<TState> OnExit(TState state, Action callback)
    {
        stateMachine.OnExit(state, callback);
        return this;
    }

    // 우선순위 설정
    public FSM<TState> SetPriority(TState state, int priority)
    {
        priorities[state] = priority;
        return this;
    }

    // 플래그 기반 상태 전이 (이벤트 리스너 등록 포함)
    public FSM<TState> DriveByFlags<TFlag>(
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
        RequestByPriority(next);
    }

    // 상태 전이 직접 요청
    public void Request(TState next)
    {
        stateMachine.Request(next);
    }

    // 다중 후보 중 가장 높은 우선순위 요청
    public void RequestByPriority(params TState[] candidates)
    {
        TState? best = default;
        int bestPriority = int.MinValue;

        foreach (var state in candidates)
        {
            if (!stateMachine.CanTransitTo(state)) continue;

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

    // 단일 후보 상태에 대한 조건부 전이
    public void RequestByPriority(TState single)
    {
        RequestByPriority(new[] { single });
    }

    // 상태 전이 로그 출력
    public FSM<TState> LogTransitions(string tag = "[FSM]")
    {
        State.AddListener(state => Debug.Log($"{tag} State → {state}"));
        return this;
    }
}