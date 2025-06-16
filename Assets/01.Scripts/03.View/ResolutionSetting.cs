using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionSetting : MonoBehaviour
{
    [Header("Resolution UI")]
    public TMP_Dropdown resolutionDropDown;
    public Toggle fullScreenBtn;
    
    private FullScreenMode fullScreenMode;                              // 전체화면
    //private List<Resolution> resolutions = new List<Resolution>();      // (현재 모니터에서 가능한) 해상도를 담아두는 리스트, 커스텀 해상도 사용 시 미사용
    private int resolutionNum;                                          // 해상도 인덱스

    // 커스텀 해상도 리스트
    private readonly List<Vector2Int> customResolutions = new List<Vector2Int>
    {
        new Vector2Int(1920, 1080),   // 16:9
        new Vector2Int(1600, 900),    // 16:9
        new Vector2Int(1280, 720),    // 16:9
        new Vector2Int(1024, 576),    // 16:9

        new Vector2Int(1920, 1200),   // 16:10
        new Vector2Int(1680, 1050),   // 16:10
        new Vector2Int(1280, 800),    // 16:10

        new Vector2Int(2560, 1080),   // 21:9 
        new Vector2Int(3440, 1440),   // 21:9

        new Vector2Int(1080, 1920),   // 모바일 (세로)
        new Vector2Int(720, 1280),    // 모바일 (세로)
        new Vector2Int(1280, 720),    // 모바일 (가로)
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
        resolutionDropDown.options.Clear();

        for (int i = 0; i < customResolutions.Count; i++)
        {
            var res = customResolutions[i];
            string ratio = GetAspectRatio(res);
            var option = new TMP_Dropdown.OptionData($"{res.x}x{res.y} ({ratio})");
            resolutionDropDown.options.Add(option);

            if (res.x == Screen.width && res.y == Screen.height)
            {
                resolutionDropDown.value = i;
                resolutionNum = i;
            }
        }
        
        // resolutions.Clear(); // 기존 해상도 초기화
        // HashSet<string> addedResolutions = new HashSet<string>(); // 문자열 해시셋 생성
        //
        // // 현재 모니터에서 지원하는 해상도 탐색, 중복되지 않은 해상도를 리스트에 추가
        // foreach (var res in Screen.resolutions)
        // {
        //     string resString = res.width + "x" + res.height;
        //     if (!addedResolutions.Contains(resString))
        //     {
        //         resolutions.Add(res);
        //         addedResolutions.Add(resString);
        //     }
        // }
        //
        // resolutionDropDown.options.Clear(); // 드롭다운 항목 초기화
        //
        // // 해상도 리스트를 돌면서 텍스트를 드롭다운 항목으로 추가
        // for (int i = 0; i < resolutions.Count; i++)
        // {
        //     var res = resolutions[i];
        //     var option = new TMP_Dropdown.OptionData($"{res.width}x{res.height}");
        //     resolutionDropDown.options.Add(option);
        //
        //     // 현재 해상도와 같은 항목이 있다면 선택된 인덱스로 설정?
        //     if (res.width == Screen.width && res.height == Screen.height)
        //     {
        //         resolutionDropDown.value = i;
        //         resolutionNum = i;
        //     }
        // }
        resolutionDropDown.RefreshShownValue();

        // 현재 모드가 전체화면인지 확인해서 토글에 반영
        fullScreenBtn.isOn = Screen.fullScreen;
        // 전체화면 모드를 변수에 저장
        fullScreenMode = Screen.fullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed; 
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
        var res = customResolutions[x];
        Debug.Log($"해상도 선택: {res.x}x{res.y}");
        // Debug.Log($"해상도 선택: {resolutions[x].width}x{resolutions[x].height}");
    }

    /// <summary>
    /// 전체화면 설정값 변경
    /// </summary>
    /// <param name="isFull"></param>
    public void FullScreenBtn(bool isFull)
    {
        fullScreenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        string modeText = isFull ? "전체화면" : "창모드";
        Debug.Log($"[화면 모드 변경] 현재 모드: {modeText} ({fullScreenMode})");
    }

    
    // 적용 버튼 클릭 시 해상도 및 전체화면 적용
    public void CheckBtnClick()
    {
        var selectedRes = customResolutions[resolutionNum];
        Screen.SetResolution(selectedRes.x, selectedRes.y, fullScreenMode);
        Debug.Log($"해상도 적용됨: {selectedRes.x}x{selectedRes.y}, 모드: {fullScreenMode}");
        // var selectedRes = resolutions[resolutionNum];
        // Screen.SetResolution(selectedRes.width, selectedRes.height, fullScreenMode);
        // Debug.Log($"해상도 적용됨: {selectedRes.width}x{selectedRes.height}, 모드: {fullScreenMode}");
        
        // InitUI(); // 해상도 바뀐 후 상태 다시 반영
    }
    
    /// <summary>
    /// 해상도 비율 계산용
    /// </summary>
    private string GetAspectRatio(Vector2Int res)
    {
        float ratio = (float)res.x / res.y;
        if (Mathf.Approximately(ratio, 16f / 9f)) return "16:9";
        if (Mathf.Approximately(ratio, 16f / 10f)) return "16:10";
        if (Mathf.Approximately(ratio, 21f / 9f)) return "21:9";
        if (Mathf.Approximately(ratio, 9f / 16f)) return "9:16";
        return "기타";
    }
}
