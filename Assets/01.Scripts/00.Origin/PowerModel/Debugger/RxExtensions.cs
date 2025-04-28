using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Extension methods for Rx framework objects
/// </summary>
public static class RxExtensions
{
    /// <summary>
    /// Record a subscription from this RxBase to a subscriber object
    /// This method is used for memory tracking and debugging
    /// </summary>
    public static void RecordSubscription(this RxBase rxBase, object subscriber)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD || RXDEBUG
        if (rxBase == null || subscriber == null)
            return;

        // Record subscription relationship in the memory tracker
        RxMemoryTracker.TrackSubscription(rxBase, subscriber);

        // Only record for cyclic detection if subscriber is RxBase
        if (subscriber is RxBase rxSubscriber)
        {
            RxCyclicDetector.RecordSubscription(rxBase, rxSubscriber);
        }
#endif
    }

    /// <summary>
    /// Remove a subscription record
    /// </summary>
    public static void RemoveSubscriptionRecord(this RxBase rxBase, object subscriber)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD || RXDEBUG
        if (rxBase == null || subscriber == null)
            return;

        // Only remove from cyclic detector if subscriber is RxBase
        if (subscriber is RxBase rxSubscriber)
        {
            RxCyclicDetector.RemoveSubscription(rxBase, rxSubscriber);
        }
#endif
    }

    /// <summary>
    /// Track this RxBase object for memory leaks
    /// </summary>
    public static T Track<T>(this T rxBase, string context = null) where T : RxBase
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD || RXDEBUG
        RxMemoryTracker.TrackObject(rxBase, context);
#endif
        return rxBase;
    }

    /// <summary>
    /// Updates RxVar to add tracking when adding listeners
    /// </summary>
    public static RxVar<T> WithTracking<T>(this RxVar<T> rxVar, string context = null)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD || RXDEBUG
        RxTrackableInjector.InjectTrackingIntoRxVar(rxVar, context);
#endif
        return rxVar;
    }

    /// <summary>
    /// Updates FSM to add tracking when adding listeners
    /// </summary>
    public static FSM<T> WithTracking<T>(this FSM<T> fsm, string context = null) where T : Enum
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD || RXDEBUG
        RxTrackableInjector.InjectTrackingIntoFSM(fsm, context);
#endif
        return fsm;
    }

    /// <summary>
    /// Updates RxModBase to add diagnostic info like source of modifications
    /// </summary>
    public static T WithSourceTracking<T, TValue>(this T rxMod) where T : RxModBase<TValue>
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD || RXDEBUG
        // Implementation depends on what you want to track
        // Could record stack trace when modifications happen
#endif
        return rxMod;
    }

    /// <summary>
    /// Enable runtime performance analysis for the RxBase object
    /// </summary>
    public static T WithPerformanceTracking<T>(this T rxBase) where T : RxBase
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD || RXDEBUG
        // Implementation would record notification timing
#endif
        return rxBase;
    }
}