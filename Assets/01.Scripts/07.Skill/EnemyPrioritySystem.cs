using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyPrioritySystem
{
    // 적 타입 우선순위 정의 (값이 낮을수록 우선순위 높음)
    private static readonly Dictionary<AtkType, int> typePriority = new Dictionary<AtkType, int>
    {
        { AtkType.shooter, 1 },
        { AtkType.tank, 1 },
        { AtkType.boss, 2 },
        { AtkType.standard, 3 },
        { AtkType.dasher, 3 },
        { AtkType.rusher, 3 }
    };

    // 적 타입의 우선순위 값 반환
    public static int GetPriority(AtkType type)
    {
        if (typePriority.TryGetValue(type, out int priority))
            return priority;
        return int.MaxValue; // 정의되지 않은 타입은 가장 낮은 우선순위
    }

    // 타입 A가 타입 B보다 높은 우선순위를 가지는지 확인
    public static bool HasHigherPriority(AtkType typeA, AtkType typeB)
    {
        return GetPriority(typeA) <= GetPriority(typeB);
    }
}