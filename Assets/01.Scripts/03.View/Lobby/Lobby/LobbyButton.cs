using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyButton : MonoBehaviour
{
    [SerializeField] private GameObject stageSelectWindow;
    [SerializeField] private Transform canvas;

    public void OpenStageSelectWindow()
    {
        var StageSelectWindow = Instantiate(stageSelectWindow, canvas);
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
