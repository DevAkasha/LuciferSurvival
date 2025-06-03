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
        if (StageManager.Instance.thisStage == null)
            return;
        if(titleText ==null)
            return;
        if(rewardText == null)
            return;

        titleText.text = $"{StageManager.Instance.thisStage.Title} 격파";
        rewardText.text = $"{StageManager.Instance.thisStage.Reward1}을/를 {StageManager.Instance.thisStage.Reward1Count}개 획득하셨습니다";
        GameManager.Instance.AddEssence(StageManager.Instance.thisStage.Reward1Count);
    }

    public void ToLobby()
    {
        Debug.Log("씬로드 로비 시작");
        WaveManager.Instance.spawnCount.Clear();
        SceneManager.LoadScene(SceneName.LobbyScene.ToString());
        Debug.Log("씬로드 로비 완료");
    }

    public void Retry()
    {
        Debug.Log("씬로드 재시작 시작");
        WaveManager.Instance.spawnCount.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Destroy(gameObject);
        Debug.Log("씬로드 재시작 완료");
    }
    
    public void NextStage()
    {
        GameManager.Instance.stageNumber += 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Destroy(gameObject);
    }
}
