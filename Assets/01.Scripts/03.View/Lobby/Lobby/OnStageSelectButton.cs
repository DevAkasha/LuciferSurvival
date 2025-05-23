using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnStageSelectButton : MonoBehaviour
{
    [SerializeField] private GameObject stageSelectWindow;
    [SerializeField] private Transform canvas;

    public void OpenStageSelectWindow()
    {
        var StageSelectWindow = Instantiate(stageSelectWindow, canvas);
    }
}
