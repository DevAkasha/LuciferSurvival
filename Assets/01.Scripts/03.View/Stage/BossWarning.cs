using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class BossWarning : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bossWarningText;

    private void OnEnable()
    {
        GetBossName();
        DestroyThis();
    }

    private void GetBossName()
    {
        bossWarningText.text = $"{StageManager.Instance.thisStage.Title} 등장";
    }

    private async void DestroyThis()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(5), DelayType.DeltaTime, PlayerLoopTiming.Update);
        Destroy(gameObject);
    }
}
