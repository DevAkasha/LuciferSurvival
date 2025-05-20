using System.Collections;
using System.Collections.Generic;
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
        if (GameManager.Instance.gameWave == null)
            return;

        for(int i = 0; i < GameManager.Instance.gameWave.Count; i++)
        {
            StageCard NextStageCard = Instantiate(stageCard, content);
            NextStageCard.GetStageInfo(i);
        }
    }
}
