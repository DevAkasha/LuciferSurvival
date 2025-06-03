using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StagePauseBtn : MonoBehaviour
{
    [SerializeField] public Button button;

    [SerializeField] private GameObject StageOptionUI;
    [SerializeField] private Transform canvas;

    public void OpenWindow()
    {
        canvas = StageUIManager.Instance.GetCanvasTransform();

        GameManager.Instance.PauseGame(0f);
        StageUIManager.Instance.GetCanvasTransform();
        var StageSelectWindow = Instantiate(StageOptionUI, canvas);
    }
}
