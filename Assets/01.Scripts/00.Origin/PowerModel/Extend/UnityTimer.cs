using System;
using System.Collections.Generic;
using UnityEngine;

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
        foreach (var task in tasks)
            task.Update(deltaTime);
    }
}

public class TimerTask
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