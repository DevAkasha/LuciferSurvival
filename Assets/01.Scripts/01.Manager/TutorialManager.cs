using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ResetPlayerPrefs : MonoBehaviour
{
    // 임시로 초기화용, window 들어가셔서 초기화 버튼 누르시면 다시 뜹니다
    [MenuItem("Window/PlayerPrefs 초기화")]
    private static void ResetPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("PlayerPrefs has been reset.");
    }
}

public class TutorialManager : Singleton<TutorialManager>
{
    protected override bool IsPersistent => false;

    [SerializeField] private GameObject[] pages; // 페이지들
    [SerializeField] private GameObject tutorial;
    [SerializeField] private Button nextBtn;
    [SerializeField] private Button prevBtn;

    private int currentPage = 0;

    private new void Awake()
    {
        if (PlayerPrefs.GetInt("TutorialShown", 0) == 1)
        {
            gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        ShowPage(0);
        nextBtn.onClick.AddListener(NextPage);
        prevBtn.onClick.AddListener(PrevPage);
    }

    private void ShowPage(int index)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == index);
        }

        currentPage = index;
        prevBtn.gameObject.SetActive(currentPage > 0);
        nextBtn.gameObject.SetActive(true);
    }

    public void NextPage()
    {
        if (currentPage < pages.Length - 1)
        {
            ShowPage(currentPage + 1);
        }
        else
        {
            EndTutorial();
        }
    }

    public void PrevPage()
    {
        if (currentPage > 0)
        {
            ShowPage(currentPage - 1);
        }
    }

    private void EndTutorial()
    {
        // 튜토리얼 UI 비활성화
        tutorial.SetActive(false);

        // 첫 실행 여부 저장
        PlayerPrefs.SetInt("TutorialShown", 1);
    }
}
