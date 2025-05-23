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

    public void QuitGame()
    {
#if UNITY_EDITOR
        // 유니티 에디터에서 실행 중일 때는 에디터 멈춤
        UnityEditor.EditorApplication.isPlaying = false;
#else
    // 빌드된 게임에서는 애플리케이션 종료
    Application.Quit();
#endif
    }
}
