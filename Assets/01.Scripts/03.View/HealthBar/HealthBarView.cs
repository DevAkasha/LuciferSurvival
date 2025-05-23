using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class HealthBarView : MonoBehaviour
{
    [SerializeField] private Image fill;
    [SerializeField] private Vector3 worldOffset;
    [SerializeField] private AngelController target;
    [SerializeField] private BossController bossTarget;
    [SerializeField] private GameObject targetGO;
    [SerializeField] private GameObject healthBar;

    [SerializeField] private bool targetDying;
    private Camera cam;
    private Action<float> hpListener;
    private Action<bool> deathListener;

    public void Init(AngelController angel)
    {
        // 기존 연결 정리
        CleanupListeners();
        target = angel;
        bossTarget = null;
        targetGO = target.gameObject;
        cam = Camera.main;

        // HealthBar 활성화
        healthBar.SetActive(true);

        // HP 초기화 및 리스너 등록
        SetupHealthListeners();
    }

    public void Init(BossController boss)
    {
        // 기존 연결 정리
        CleanupListeners();
        bossTarget = boss;
        target = null;
        targetGO = bossTarget.gameObject;
        cam = Camera.main;

        // HealthBar 활성화
        healthBar.SetActive(true);

        // HP 초기화 및 리스너 등록
        SetupHealthListeners();
    }

    private void SetupHealthListeners()
    {
        if (target != null)
        {
            fill.fillAmount = target.Entity.Model.NormalizedHP.Value;
            hpListener = v => fill.fillAmount = v;
            target.Entity.Model.NormalizedHP.AddListener(hpListener);

            deathListener = isDead => { if (isDead) OnTargetDeath(); };
            target.Entity.Model.Flags.AddListener(PlayerStateFlag.Death, deathListener);
        }
        else if (bossTarget != null)
        {
            fill.fillAmount = bossTarget.Entity.Model.NormalizedHP.Value;
            hpListener = v => fill.fillAmount = v;
            bossTarget.Entity.Model.NormalizedHP.AddListener(hpListener);

            deathListener = isDead => { if (isDead) OnTargetDeath(); };
            bossTarget.Entity.Model.Flags.AddListener(PlayerStateFlag.Death, deathListener);
        }
    }

    private void LateUpdate()
    {
        // 타겟 유효성 검사 및 처리
        if (target != null)
        {
            HandleAngelTarget();
        }
        else if (bossTarget != null)
        {
            HandleBossTarget();
        }
    }

    private void HandleAngelTarget()
    {
        // 월드 좌표를 스크린 좌표로 변환
        transform.position = cam.WorldToScreenPoint(target.Entity.headPivot.position + worldOffset);

        // 타겟이 비활성화된 경우 헬스바도 비활성화
        if (!targetGO.activeSelf && healthBar.activeSelf )
        {
            Detach();
        }
        else if (targetGO.activeSelf && healthBar.activeSelf)
        {
            healthBar.SetActive(true);
            CleanupListeners();
            SetupHealthListeners();
        }
    }

    private void HandleBossTarget()
    {
        // 월드 좌표를 스크린 좌표로 변환
        transform.position = cam.WorldToScreenPoint(bossTarget.Entity.headPivot.position + worldOffset);

        // 타겟이 비활성화된 경우 헬스바도 비활성화
        if (!targetGO.activeSelf && healthBar.activeSelf)
        {
            Detach();
        }
        else if(targetGO.activeSelf && !healthBar.activeSelf)
        {
            healthBar.SetActive(true);
            CleanupListeners();
            SetupHealthListeners();
        }
    }

    private void CleanupListeners()
    {
        if (hpListener != null)
        {
            if (target != null)
            {
                target.Entity.Model.NormalizedHP.RemoveListener(hpListener);
                target.Entity.Model.Flags.RemoveListener(PlayerStateFlag.Death, deathListener);
            }
            if (bossTarget != null)
            {
                bossTarget.Entity.Model.NormalizedHP.RemoveListener(hpListener);
                bossTarget.Entity.Model.Flags.RemoveListener(PlayerStateFlag.Death, deathListener);
            }
            hpListener = null;
            deathListener = null;
        }
    }

    private void OnDisable()
    {
        CleanupListeners();
    }

    private void OnTargetDeath()
    {
        targetDying = true;
        this.DelayedCall(1.5f, TryDetach);
    }
    public void TryDetach()
    {  
        if (targetDying) Detach();
        targetDying = false;
    }
    public void Detach()
    {
        CleanupListeners();
        healthBar.SetActive(false);
        target = null;
        bossTarget = null;
        targetGO = null;
        targetDying = false;
        HealthBarManager.Instance.Detach(this);
    }
}