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

    public Transform? GetCurrentTarget()
    {
        return curTarget;
    }

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

        // 현재 타겟 유효성 체크
        if (curTarget != null)
        {
            float sqrDist = (curTarget.position - transform.position).sqrMagnitude;
            if (sqrDist < Model.range * Model.range) return;

            curTarget = null; // 범위 벗어남 → 무효 처리
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
            return;
        }

        closestDist = float.MaxValue;
        closest = null;

        for (int i = 0; i < count; i++)
        {
            Collider col = colliders[i];
            if (col == null) continue;

            float dist = (col.transform.position - transform.position).sqrMagnitude;
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = col.transform;
            }
        }

        curTarget = closest;

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
