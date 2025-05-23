using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class StageSelectArea : MonoBehaviour
{
    [SerializeField] private StageCard stageCard;
    [SerializeField] private Transform content;

    private void Start()
    {
        SetStageCard();
    }

    public async void SetStageCard()
    {
        if (GameManager.Instance.allStageList == null)
        {
            return;
        }

        await UniTask.Delay(TimeSpan.FromSeconds(0.1f), DelayType.DeltaTime, PlayerLoopTiming.Update);

        for (int i = 0; i < GameManager.Instance.allStageList.Count; i++)
        {
            StageCard NextStageCard = Instantiate(stageCard, content);
            NextStageCard.GetStageInfo(i);
        }
    }
}
