using System;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public sealed class RxVar<T> : RxBase, IRxReadable<T>
{
    private T value;
    private readonly List<Action<T>> listeners = new();

    public RxVar(T initialValue = default, object owner = null)
    {
        value = initialValue;
        if (owner is ITrackableRxModel model)
        {
            model.RegisterRx(this); // Rx 필드를 모델에 등록
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD || RXDEBUG
        // Auto-register with memory tracker
        RxMemoryTracker.TrackObject(this, $"RxVar<{typeof(T).Name}> owned by {owner?.GetType().Name ?? "unknown"}");
#endif
    }

    public T Value => value;

    public void SetValue(T newValue) // 값 설정
    {
        if (!EqualityComparer<T>.Default.Equals(value, newValue))
        {
            value = newValue;
            NotifyAll();
        }
    }

    public void AddListener(Action<T> listener) // 값 변경을 구독할 수 있음
    {
        if (listener != null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD || RXDEBUG
            // Check for self-subscription which can cause infinite loops
            if (listener.Target == this)
            {
                Debug.LogWarning($"[RxVar] Self-subscription detected in {this}! This may cause infinite loops.");
            }

            // Record the subscription for debugging
            this.RecordSubscription(listener.Target);
#endif

            listeners.Add(listener);
            listener(value);
        }
    }

    public void RemoveListener(Action<T> listener) // 구독 해제
    {
        if (listener != null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD || RXDEBUG
            // Remove subscription tracking
            this.RemoveSubscriptionRecord(listener.Target);
#endif
            listeners.Remove(listener);
        }
    }

    public override void ClearRelation()
    {
        listeners.Clear();
    }

    private void NotifyAll()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD || RXDEBUG
        // Start performance measurement
        RxDebugger.BeginNotification();
        RxPerformanceMonitor.StartMeasurement();
#endif

        // Cache the list count since listeners may change during notification
        int listenerCount = listeners.Count;

        // Take a snapshot of the current listeners to avoid modification issues
        var currentListeners = new Action<T>[listenerCount];
        listeners.CopyTo(currentListeners);

        // Notify all listeners
        foreach (var listener in currentListeners)
        {
            if (listener != null) // Check in case it was removed during iteration
            {
                try
                {
                    listener(value);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[RxVar] Exception in listener: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD || RXDEBUG
        // End performance measurement
        RxDebugger.RecordNotification(this, 0, listenerCount); // Duration is calculated by RxPerformanceMonitor
#endif
    }

    public override string ToString()
    {
        return $"RxVar<{typeof(T).Name}>({value})";
    }
}