using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionSetting : MonoBehaviour
{
    FullScreenMode fullScreenMode;
    public TMP_Dropdown resolutionDropDown;
    public Toggle fullScreenBtn;
    List<Resolution> resolutions = new List<Resolution>();
    int resolutionNum; 

    void Start()
    {
        InitUI();
    }
    
    void InitUI()
    {
        resolutions.Clear();
        HashSet<string> addedResolutions = new HashSet<string>();

        foreach (var res in Screen.resolutions)
        {
            string resString = res.width + "x" + res.height;

            if (!addedResolutions.Contains(resString))
            {
                resolutions.Add(res);
                addedResolutions.Add(resString);
            }
        }

        resolutionDropDown.options.Clear();

        int optionNum = 0;
        for (int i = 0; i < resolutions.Count; i++)
        {
            var option = new TMP_Dropdown.OptionData();
            option.text = resolutions[i].width + "x" + resolutions[i].height;
            resolutionDropDown.options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                resolutionDropDown.value = optionNum;
            }

            optionNum++;
        }

        resolutionDropDown.RefreshShownValue();

        // 현재 모드가 풀스크린인지 확인해서 토글에 반영
        fullScreenBtn.isOn = Screen.fullScreenMode == FullScreenMode.FullScreenWindow;
    }


    // void InitUI()
    // {
    //     for (int i = 0; i < Screen.resolutions.Length; i++)
    //     {
    //         if (Screen.resolutions[i].refreshRateRatio.value == 60)
    //         {
    //             resolutions.Add(Screen.resolutions[i]);
    //         }
    //     }
    //     resolutionDropDown.options.Clear();
    //
    //     int optionNum = 0;
    //     foreach (Resolution item in resolutions)
    //     {
    //         TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
    //         option.text = item.width + "x" + item.height + " " + item.refreshRateRatio.value + "hz"; // 너비, 높이, 주사율
    //         resolutionDropDown.options.Add(option);
    //
    //         if (item.width == Screen.width && item.height == Screen.height)
    //             resolutionDropDown.value = optionNum;
    //         optionNum++;
    //
    //         Debug.Log(item.width + "x" + item.height + " ");
    //     }
    //
    //     resolutionDropDown.RefreshShownValue();
    //
    //     fullScreenBtn.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow) ? true : false; 
    // }

    public void DropboxOptionChange(int x)
    {
        resolutionNum = x;
    }

    public void FullScreenBtn(bool isFull)
    {
        fullScreenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        
        // fullScreenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.FullScreenWindow;
    }

    public void CheckBtnClick()
    {
        Screen.SetResolution(resolutions[resolutionNum].width, resolutions[resolutionNum].height,fullScreenMode);
    }
}
