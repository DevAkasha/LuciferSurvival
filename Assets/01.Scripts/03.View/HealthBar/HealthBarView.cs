using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarView : MonoBehaviour
{
    [SerializeField] private Image fill;
    [SerializeField] private Vector3 worldOffset;

    private AngelController target;
    private Camera cam;

    private Action<float> hpListener;


    public void Init(AngelController angel)
    {
        target = angel;
        cam = Camera.main;
        hpListener = v => fill.fillAmount = v;

        target.Entity.Model.NormalizedHP.AddListener(hpListener);
        fill.fillAmount = target.Entity.Model.NormalizedHP.Value;
        target.Entity.Model.Flags.AddListener(PlayerStateFlag.Death, v => { if (v) OnTargetDeath(); });
    }

    private void LateUpdate()
    {
        if (!target) return;
        transform.position = cam.WorldToScreenPoint(target.Entity.headPivot.position + worldOffset);
    }

    private void OnDisable()
    {
        if (target != null)
        {
            target.Entity.Model.NormalizedHP.RemoveListener(hpListener);
            target = null;
        }
    }

    private void OnTargetDeath()
    {
        HealthBarManager.Instance.Detach(this);
    }
}
