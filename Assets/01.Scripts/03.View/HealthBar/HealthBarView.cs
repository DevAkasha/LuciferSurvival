using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarView : MonoBehaviour
{
    [SerializeField] private Image fill;            //체력을 표시하기 위해 조정하는 이미지
    [SerializeField] private Vector3 worldOffset;   //몹이 존재하는 월드좌표

    [SerializeField] private AngelController target;                 //헬스바의 대상
    [SerializeField] private BossController boss;                    //헬스바의 대상
    private Camera cam;                             //헬스바가 표시될 카메라
    private Action<float> hpListener;               //반응형 리스너

    public void Init(AngelController angel)
    {
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
    public void Init(BossController angel)
    {
        boss = angel;
        cam = Camera.main;
        fill.fillAmount = boss.Entity.Model.NormalizedHP.Value;   //fillAmount초기화

        hpListener = v => fill.fillAmount = v;                      //리스너의 콜백을 설정
        boss.Entity.Model.NormalizedHP.AddListener(hpListener);   //HP에 리스너를 등록

        boss.Entity.Model.Flags.AddListener(                      //타겟의 상태(플래그)에 대한 접근
            PlayerStateFlag.Death,                                  //리스너를 등록할 상태를 특정
            v => { if (v) OnTargetDeath();                          //동작할 콜백을 등록 
            });
    }

    private void LateUpdate() 
    {
        if (target!=null) 
            transform.position = cam.WorldToScreenPoint(target.Entity.headPivot.position + worldOffset);
        if (boss != null)
            transform.position = cam.WorldToScreenPoint(boss.Entity.headPivot.position + worldOffset);
    }

    private void OnDisable()
    {
        if (target != null)
        {
            target.Entity.Model.NormalizedHP.RemoveListener(hpListener);
            target = null;
        }

        if (boss != null)
        {
            boss.Entity.Model.NormalizedHP.RemoveListener(hpListener);
            boss = null;
        }
    }

    private void OnTargetDeath() => HealthBarManager.Instance.Detach(this);
}
