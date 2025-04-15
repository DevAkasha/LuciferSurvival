using System.Collections.Generic;
using System;

public class RxStateMachine<TState> where TState : Enum
{
    private readonly RxVar<TState> current;
    private readonly Dictionary<TState, Func<TState, bool>> transitionGuards = new();
    private readonly Dictionary<TState, Action> onEnter = new();
    private readonly Dictionary<TState, Action> onExit = new();

    public event Action<TState>? OnStateChanged;
    public RxVar<TState> Current => current;
    public TState Value => current.Value;

    public RxStateMachine(TState initialState)
    {
        current = new RxVar<TState>(initialState);
    }

    public void AddTransitionRule(TState to, Func<TState, bool> condition) => transitionGuards[to] = condition;

    public bool CanTransitTo(TState next)
        => !EqualityComparer<TState>.Default.Equals(Value, next)
           && (!transitionGuards.TryGetValue(next, out var rule) || rule(Value));

    public void Request(TState next)
    {
        if (!CanTransitTo(next)) return;

        onExit.TryGetValue(Value, out var exit); exit?.Invoke();

        current.SetValue(next);
        OnStateChanged?.Invoke(next);

        onEnter.TryGetValue(next, out var enter); enter?.Invoke();
    }

    public void OnEnter(TState state, Action callback)
    {
        if (!onEnter.ContainsKey(state)) onEnter[state] = callback;
        else onEnter[state] += callback;
    }

    public void OnExit(TState state, Action callback)
    {
        if (!onExit.ContainsKey(state)) onExit[state] = callback;
        else onExit[state] += callback;
    }
}
