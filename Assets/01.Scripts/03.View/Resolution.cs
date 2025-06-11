using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Resolution : MonoBehaviour
{
    public TMP_Dropdown resolutionDropDown;
    List<Resolution> resolutions = new List<Resolution>();


    public void Start()
    {
        //InitUI();
    }

    //private void InitUI()
    //{
    //    for (int i = 0; i < Screen.resolutions.Length; i++)
    //    {
    //        resolutionDropDown.Add(Screen.resolutions[i]);
    //    }
    //}



    //public void DropboxOptionChange(int x)
    //{
    //    resolutionNum = x;
    //}
}
