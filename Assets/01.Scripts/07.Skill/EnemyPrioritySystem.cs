using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyPrioritySystem
{
    // 적 타입 우선순위 정의 (값이 낮을수록 우선순위 높음)
    private static readonly Dictionary<EnemyType, int> typePriority = new Dictionary<EnemyType, int>
    {
        { EnemyType.shooter, 1 },
        { EnemyType.tank, 1 },
        { EnemyType.boss, 2 },
        { EnemyType.standard, 3 },
        { EnemyType.dasher, 3 },
        { EnemyType.rusher, 3 }
    };

    // 적 타입의 우선순위 값 반환
    public static int GetPriority(EnemyType type)
    {
        if (typePriority.TryGetValue(type, out int priority))
            return priority;
        return int.MaxValue; // 정의되지 않은 타입은 가장 낮은 우선순위
    }

    // 타입 A가 타입 B보다 높은 우선순위를 가지는지 확인
    public static bool HasHigherPriority(EnemyType typeA, EnemyType typeB)
    {
        return GetPriority(typeA) <= GetPriority(typeB);
    }
}