using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StageCard : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemySprite;
    [SerializeField] private TextMeshProUGUI whatStageText;
    [SerializeField] private TextMeshProUGUI rewardCountText;
    private TextMeshProUGUI enemyTypeText;
    private StageModel stageModel;

    public int StageNumber;

    public void GetStageInfo(int stageNumber)
    {
        StageNumber = stageNumber;

        stageModel = StageManager.Instance.allStageList[StageNumber];
        whatStageText.text = $"스테이지{StageNumber + 1}";
        WriteEnemyType();
        rewardCountText.text = stageModel.Reward1Count.ToString();
    }

    private void WriteEnemyType()
    {
        for(int i = 0 ; i < enemySprite.Count; i++)
        {
            if (stageModel.StageEnemyType[i] == null)
            {
                enemySprite[i].gameObject.SetActive(false);
                continue;
            }
            
            //나중에 썸네일 추가 기능 추가 필요
            enemyTypeText = enemySprite[i].GetComponentInChildren<TextMeshProUGUI>();
            enemyTypeText.text = stageModel.StageEnemyType[i];
        }
    }

    public void WaveDataSet()
    {
        StageManager.Instance.SetStage(StageNumber);
    }
}
