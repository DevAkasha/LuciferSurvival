using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SceneName
{
    LobbyScene,
    StageScene
}

public class SceneButton : MonoBehaviour
{
    private Button button;

   
    [SerializeField] private SceneName targetScene;

    [SerializeField] private bool disableAfterClick = true; // 중복 클릭 방지
    [SerializeField] private float disableDuration = 1f;    // 비활성화 시간

    private void Awake()
    {
        button = GetComponent<Button>();

        // 버튼이 없으면 경고
        if (button == null)
        {
            Debug.LogError($"Button component not found on {gameObject.name}!");
            return;
        }

        // 온클릭 이벤트 등록
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnDestroy()
    {
        // 메모리 누수 방지
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
    }
    private void OnButtonClick()
    {
        MoveScene(targetScene);
    }

    public void MoveScene(SceneName sceneName)
    {
        // 현재 씬과 같으면 무시
        string currentSceneName = SceneManager.GetActiveScene().name;
        string targetSceneName = sceneName.ToString();

        if (currentSceneName == targetSceneName)
        {
            Debug.Log($"Already in {targetSceneName}");
            return;
        }

        Debug.Log($"Loading scene: {targetSceneName}");

        // 중복 클릭 방지
        if (disableAfterClick)
        {
            button.interactable = false;
            StartCoroutine(ReEnableButton());
        }

        // 씬 로드
        SceneManager.LoadScene(targetSceneName);
    }


    private IEnumerator ReEnableButton()
    {
        yield return new WaitForSeconds(disableDuration);

        if (button != null)
        {
            button.interactable = true;
        }
    }

    #region Editor Helper Methods
#if UNITY_EDITOR

    private void Reset()
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogWarning($"No Button component found on {gameObject.name}. Please add a Button component.");
        }
    }
#endif
    #endregion
}