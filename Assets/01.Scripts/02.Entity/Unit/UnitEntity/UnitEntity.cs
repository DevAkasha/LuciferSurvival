using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class UnitEntity : BaseEntity<UnitModel>
{
    [SerializeField]
    private string rcode;

    [SerializeField]
    private Projectile projectile;

    private Collider[] colliders = new Collider[10];

    public Transform? curTarget;
    public AtkType curTargetType;

    protected override void SetupModel()
    {
        if(string.IsNullOrEmpty(rcode)) return;
        Model = new UnitModel(DataManager.Instance.GetData<UnitDataSO>(rcode));
    }

    public void FindEnemy()
    {
        int count = 0;
        float closestDist = float.MaxValue;
        Transform closest = null;
        AtkType closestType = AtkType.none;

        // 현재 타겟 유효성 체크
        if (curTarget != null)
        {
            float sqrDist = (curTarget.position - transform.position).sqrMagnitude;
            if (sqrDist < Model.range * Model.range) return;

            curTarget = null; // 범위 벗어남 → 무효 처리
            curTargetType = AtkType.none;
        }

        // 실제 탐색
        count = Physics.OverlapSphereNonAlloc(
            transform.position,
            Model.range,
            colliders,
            1 << LayerMask.NameToLayer("Enemy")
        );

        if (count == 0)
        {
            curTarget = null;
            curTargetType = AtkType.none;
            return;
        }

        // 우선순위 사용 여부 확인
        bool usePriority = Model.usePriorityTargeting;
        int highestPriority = int.MaxValue;

        closestDist = float.MaxValue;
        closest = null;

        for (int i = 0; i < count; i++)
        {
            Collider col = colliders[i];
            if (col == null) continue;

            // 우선순위 기반 타겟팅 사용 시
            if (usePriority)
            {
                // 적의 AtkType 가져오기
                AngelEntity angelEnemy = col.GetComponent<AngelEntity>();
                BossEntity bossEnemy = col.GetComponent<BossEntity>();

                AtkType enemyType = AtkType.standard; // 기본값

                if (angelEnemy != null && angelEnemy.Model != null)
                {
                    enemyType = angelEnemy.Model.atkType;
                }
                else if (bossEnemy != null)
                {
                    enemyType = AtkType.boss; // 보스는 항상 보스 타입
                }

                int enemyPriority = EnemyPrioritySystem.GetPriority(enemyType);
                float dist = (col.transform.position - transform.position).sqrMagnitude;

                // 1) 우선순위가 더 높은 적 발견
                if (enemyPriority < highestPriority)
                {
                    highestPriority = enemyPriority;
                    closestDist = dist;
                    closest = col.transform;
                    closestType = enemyType;
                }
                // 2) 같은 우선순위 내에서는 거리가 가까운 적 선택
                else if (enemyPriority == highestPriority && dist < closestDist)
                {
                    closestDist = dist;
                    closest = col.transform;
                    closestType = enemyType;
                }
            }
            // 기존 방식: 가장 가까운 적 찾기
            else
            {
                float dist = (col.transform.position - transform.position).sqrMagnitude;
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = col.transform;
                }
            }
        }

        curTarget = closest;
        curTargetType = closestType;

        // 대상을 발견했다면 공격 상태로 전환
        if (curTarget != null)
            Model.unitState = eUnitState.Attack;
    }

    public void AttackEnemy()
    {
        //가장 가까운 적을 찾아 공격하는 로직 구현
        //공격이 끝나면 Stay 상태 변환 필요
        if (curTarget != null)
        {
            Shoot(curTarget);
            //공격 후 collider를 비워 새로운 적을 찾게 만듦
            //공격이 끝난 후 Stay값으로 변경하여 공격이 끝났음을 보여주는 처리
            Array.Clear(colliders, 0, colliders.Length);
        }
        Model.unitState = eUnitState.Stay;
    }

    public void Shoot(Transform target)
    {
        Projectile bullet = Instantiate(projectile, transform.position, Quaternion.identity);
        bullet.Init(1f, 1, Model.atk, target);
    }
}
