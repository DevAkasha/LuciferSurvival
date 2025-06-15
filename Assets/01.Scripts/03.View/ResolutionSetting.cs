using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionSetting : MonoBehaviour
{
    [Header("Resolution UI")]
    public TMP_Dropdown resolutionDropDown;
    public Toggle fullScreenBtn;
    
    //private FullScreenMode fullScreenMode = FullScreenMode.FullScreenWindow; // 기본값
    private FullScreenMode fullScreenMode;
    private List<Resolution> resolutions = new List<Resolution>();
    private int resolutionNum; 
    
    private readonly List<Vector2Int> customResolutions = new List<Vector2Int>
    {
        new Vector2Int(1920, 1080),
        new Vector2Int(1600, 900),
        new Vector2Int(1280, 720),
        new Vector2Int(1024, 576)
    };

    void Start()
    {
        InitUI(); 
        AddListeners();
    }
    
    /// <summary>
    /// 해상도 드롭다운 및 토글 초기화
    /// </summary>
    void InitUI()
    {
        resolutions.Clear();
        HashSet<string> addedResolutions = new HashSet<string>();
        
        // 현재 모니터에서 지원하는 해상도를 가져옴
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

        for (int i = 0; i < resolutions.Count; i++)
        {
            var res = resolutions[i];
            var option = new TMP_Dropdown.OptionData($"{res.width}x{res.height}");
            resolutionDropDown.options.Add(option);

            if (res.width == Screen.width && res.height == Screen.height)
            {
                resolutionDropDown.value = i;
                resolutionNum = i;
            }
        }
        resolutionDropDown.RefreshShownValue();

        // 현재 모드가 전체화면인지 확인해서 토글에 반영
        fullScreenBtn.isOn = Screen.fullScreen;
        fullScreenMode = Screen.fullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        // fullScreenBtn.isOn = Screen.fullScreenMode == FullScreenMode.FullScreenWindow;
        // fullScreenMode = Screen.fullScreenMode;
    }
    
    /// <summary>
    /// UI 이벤트 리스너 연결
    /// </summary>
    void AddListeners()
    {
        resolutionDropDown.onValueChanged.AddListener(DropboxOptionChange);
        fullScreenBtn.onValueChanged.AddListener(FullScreenBtn);
    }

    /// <summary>
    /// 드롭다운 변경 시 인덱스 저장
    /// </summary>
    /// <param name="x"></param>
    public void DropboxOptionChange(int x)
    {
        resolutionNum = x;
        Debug.Log($"해상도 선택: {resolutions[x].width}x{resolutions[x].height}");
    }

    /// <summary>
    /// 전체화면 설정값 변경
    /// </summary>
    /// <param name="isFull"></param>
    public void FullScreenBtn(bool isFull)
    {
        fullScreenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Debug.Log($"전체화면 모드 변경됨: {fullScreenMode}");
    }

    
    // 적용 버튼 클릭 시 해상도 및 전체화면 적용
    public void CheckBtnClick()
    {
        var selectedRes = resolutions[resolutionNum];
        Screen.SetResolution(selectedRes.width, selectedRes.height, fullScreenMode);
        Debug.Log($"해상도 적용됨: {selectedRes.width}x{selectedRes.height}, 모드: {fullScreenMode}");
        
        // InitUI(); // 해상도 바뀐 후 상태 다시 반영
        
    }
}
