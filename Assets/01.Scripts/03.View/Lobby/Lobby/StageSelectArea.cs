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

    public void SetStageCard()
    {
        if (GameManager.Instance.allStageList == null)
        {
            return;
        }

        for (int i = 0; i < GameManager.Instance.allStageList.Count; i++)
        {
            StageCard NextStageCard = Instantiate(stageCard, content);
            NextStageCard.GetStageInfo(i);
        }
    }

    public void QuitWindow()
    {
        Destroy(gameObject);
    }
}
