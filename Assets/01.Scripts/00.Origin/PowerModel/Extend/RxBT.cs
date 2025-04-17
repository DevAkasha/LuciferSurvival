using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class RxBehaviorNode // 행동 트리의 기본 노드 클래스
{
    public abstract bool Check();
    public abstract void Run();
}

public class DoIf : RxBehaviorNode // 조건이 true일 때 액션 실행
{
    private readonly Func<bool> condition;
    private readonly Action action;

    public DoIf(Func<bool> condition, Action action)
    {
        this.condition = condition;
        this.action = action;
    }

    public override bool Check() => condition();
    public override void Run() => action();

    public static DoIf Create(Func<bool> condition, Action action) => new(condition, action);
}

public class FirstTrue : RxBehaviorNode // 여러 노드 중 조건을 만족하는 첫 번째 노드 실행
{
    private readonly List<RxBehaviorNode> children;

    public FirstTrue(IEnumerable<RxBehaviorNode> nodes)
    {
        children = new List<RxBehaviorNode>(nodes);
    }

    public override bool Check() => children.Exists(c => c.Check());

    public override void Run()
    {
        foreach (var child in children)
        {
            if (child.Check())
            {
                child.Run();
                break;
            }
        }
    }
}

public class RunAllIfTrue : RxBehaviorNode // 모든 노드가 조건을 만족하면 모두 실행
{
    private readonly List<RxBehaviorNode> children;

    public RunAllIfTrue(IEnumerable<RxBehaviorNode> nodes) // 모든 노드가 조건을 만족하면 모두 실행
    {
        children = new List<RxBehaviorNode>(nodes);
    }

    public override bool Check() => children.TrueForAll(c => c.Check());

    public override void Run()
    {
        foreach (var child in children)
        {
            if (child.Check())
                child.Run();
        }
    }
}

public class InvertCondition : RxBehaviorNode // 조건을 반전시켜 평가
{
    private readonly RxBehaviorNode node;

    public InvertCondition(RxBehaviorNode node) // 조건을 반전시켜 평가
    {
        this.node = node;
    }

    public override bool Check() => !node.Check();
    public override void Run() => node.Run();
}

public class RunAllAlways : RxBehaviorNode // 조건과 관계없이 항상 실행
{
    private readonly List<RxBehaviorNode> children;

    public RunAllAlways(IEnumerable<RxBehaviorNode> nodes) // 조건과 관계없이 항상 실행
    {
        children = new List<RxBehaviorNode>(nodes);
    }

    public override bool Check() => true;

    public override void Run()
    {
        foreach (var child in children)
        {
            if (child.Check())
                child.Run();
        }
    }
}

public static class FSMBehaviorExtensions
{
    public static FSM<TState> DriveByBehavior<TState>(
        this FSM<TState> fsm,
        Func<RxBehaviorNode> tree,
        float interval = 0.2f)
        where TState : Enum
    {
        UnityTimer.ScheduleRepeating(interval, () =>
        {
            var root = tree();
            if (root.Check())
                root.Run();
        });
        return fsm;
    }
}

// 유니티 타이머 유틸 (MonoBehaviour 없이 처리 가능하도록 별도 구현 필요)
public static class UnityTimer // MonoBehaviour 없이 타이머 구현
{
    private static readonly List<TimerTask> tasks = new();

    public static void ScheduleRepeating(float interval, Action callback)
    {
        tasks.Add(new TimerTask(interval, callback));
    }

    public static void Tick(float deltaTime)
    {
        Debug.Log("run Tick");
        foreach (var task in tasks)
            task.Update(deltaTime);
    }

    private class TimerTask
    {
        private readonly float interval;
        private readonly Action callback;
        private float time;

        public TimerTask(float interval, Action callback)
        {
            this.interval = interval;
            this.callback = callback;
        }

        public void Update(float delta)
        {
            time += delta;
            if (time >= interval)
            {
                time = 0f;
                callback();
            }
        }
    }
}
