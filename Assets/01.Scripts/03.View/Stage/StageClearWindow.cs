using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageClearWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI rewardText;

    private void OnEnable()
    {
        GetTitle();
    }

    private void GetTitle()
    {
        if (GameManager.Instance.thisStage == null)
            return;
        if(titleText ==null)
            return;
        if(rewardText == null)
            return;

        titleText.text = $"{GameManager.Instance.thisStage.Title} 격파";
        rewardText.text = $"{GameManager.Instance.thisStage.Reward1}을/를 {GameManager.Instance.thisStage.Reward1Count}개 획득하셨습니다";
        GameManager.Instance.AddEssence(GameManager.Instance.thisStage.Reward1Count);
    }

    public void ToLobby()
    {
        SceneManager.LoadScene(SceneName.LobbyScene.ToString());
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Destroy(gameObject);
    }
    
    public void NextStage()
    {
        int nextStageNumber = GameManager.Instance.stageNumber + 1;
        GameManager.Instance.SetStage(nextStageNumber);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Destroy(gameObject);
    }
}
