using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageOptionBtn : MonoBehaviour
{
    [SerializeField] private GameObject StageOptionUI;
    [SerializeField] private Transform canvas;

    public void OpenWindow()
    {
        canvas = StageUIManager.Instance.GetCanvasTransform();
        var StageSelectWindow = Instantiate(StageOptionUI, canvas);
    }

    public void StageLobby()
    {
        WaveManager.Instance.spawnCount.Clear();
        SceneManager.LoadScene(SceneName.LobbyScene.ToString());
    }

    public void StageRetry()
    {
        WaveManager.Instance.spawnCount.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
        Destroy(gameObject);
    }

    public void CloseOption()
    {
        GameManager.Instance.PauseReleaseGame();
        Destroy(gameObject);
    }
}
