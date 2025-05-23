using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarView : MonoBehaviour
{
    [SerializeField] private Image fill;            //체력을 표시하기 위해 조정하는 이미지
    [SerializeField] private Vector3 worldOffset;   //몹이 존재하는 월드좌표

    [SerializeField] private AngelController target;                 //헬스바의 대상
    [SerializeField] private BossController bossTarget;                    //헬스바의 대상
    
    [SerializeField] private GameObject healthBar;           //헬스바자식

    private Camera cam;                             //헬스바가 표시될 카메라
    private Action<float> hpListener;               //반응형 리스너

    public void Init(AngelController angel)
    {
        healthBar.SetActive(true);
        target = angel;
        cam = Camera.main;
        fill.fillAmount = target.Entity.Model.NormalizedHP.Value;   //fillAmount초기화

        hpListener = v => fill.fillAmount = v;                      //리스너의 콜백을 설정
        target.Entity.Model.NormalizedHP.AddListener(hpListener);   //HP에 리스너를 등록
                                                                    
        target.Entity.Model.Flags.AddListener(                      //타겟의 상태(플래그)에 대한 접근
            PlayerStateFlag.Death,                                  //리스너를 등록할 상태를 특정
            v => { if (v) OnTargetDeath();                          //동작할 콜백을 등록 
            });
    }

    public void Init(BossController boss)
    {
        healthBar.SetActive(true);
        bossTarget = boss;
        cam = Camera.main;
        fill.fillAmount = bossTarget.Entity.Model.NormalizedHP.Value;   //fillAmount초기화

        hpListener = v => fill.fillAmount = v;                      //리스너의 콜백을 설정
        bossTarget.Entity.Model.NormalizedHP.AddListener(hpListener);   //HP에 리스너를 등록

        bossTarget.Entity.Model.Flags.AddListener(                      //타겟의 상태(플래그)에 대한 접근
            PlayerStateFlag.Death,                                  //리스너를 등록할 상태를 특정
            v => { if (v) OnTargetDeath();                          //동작할 콜백을 등록 
            });
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            transform.position = cam.WorldToScreenPoint(target.Entity.headPivot.position + worldOffset);
            if (target.gameObject.activeSelf && !healthBar.activeSelf)
            {
                Init(target);

                if (!target.gameObject.activeSelf && healthBar.activeSelf)
                {
                    healthBar.SetActive(false);
                }
            }

            if (bossTarget != null)
            {
                transform.position = cam.WorldToScreenPoint(bossTarget.Entity.headPivot.position + worldOffset);
                if (bossTarget.gameObject.activeSelf && !healthBar.activeSelf)
                {
                    Init(bossTarget);
                }
                if (!bossTarget.gameObject.activeSelf && healthBar.activeSelf)
                {
                    healthBar.SetActive(false);
                }
            }
        }
    }

    private void OnDisable()
    {
        if (target != null)
        {
            target.Entity.Model.NormalizedHP.RemoveListener(hpListener);
            target = null;
        }

        if (bossTarget != null)
        {
            bossTarget.Entity.Model.NormalizedHP.RemoveListener(hpListener);
            bossTarget = null;
        }
    }

    private void OnTargetDeath()
    {
        UnityTimer.ScheduleRepeating(1.5f, Detach);
    }

    public void Detach() 
    {
        healthBar.SetActive(false);
        HealthBarManager.Instance.Detach(this);
    }
}
