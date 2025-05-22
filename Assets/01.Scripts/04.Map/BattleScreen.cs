using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleScreen : MonoBehaviour
{
    [Header("Battle Screen Settings")]
    public Image battleScreen;
    public float battleScreenBlinkInterval = 0.5f; // 깜빡임 간격
    public float battleScreenMinAlpha = 0.2f; // 최소 투명도
    public float battleScreenMaxAlpha = 0.3f; // 최대 투명도

    private Coroutine battleScreenRoutine; // 배틀스크린 깜빡임 코루틴

    private void OnEnable()
    {
        // 씬이 활성화될 때 배틀스크린 상태 확인
        UpdateBattleScreenState();
    }

    private void OnDisable()
    {
        // 씬이 비활성화될 때 코루틴 정리
        if (battleScreenRoutine != null)
        {
            StopCoroutine(battleScreenRoutine);
            battleScreenRoutine = null;
        }
    }

    public void UpdateBattleScreenState()
    {
        // GameObject가 비활성화된 경우 코루틴 실행 불가하므로 종료
        if (!gameObject.activeInHierarchy || battleScreen == null)
            return;

        // 기존 코루틴이 실행 중이면 중지
        if (battleScreenRoutine != null)
        {
            StopCoroutine(battleScreenRoutine);
            battleScreenRoutine = null;
        }

        // 배틀스크린 초기화
        battleScreen.color = Color.clear;

        if (TimeManager.Instance.currentTimeState == TimeState.Day)
        {
            battleScreen.enabled = true; // 이미지 표시
            battleScreenRoutine = StartCoroutine(ShowBattleScreen());
        }
        else
        {
            battleScreen.enabled = false; // 밤이면 이미지 숨김
        }
    }

    // 배틀스크린 깜빡임 효과
    private IEnumerator ShowBattleScreen()
    {
        float alpha = battleScreenMinAlpha;
        float direction = 1f; // 1이면 증가, -1이면 감소

        Color baseColor = new Color(1f, 0f, 0f);

        while (TimeManager.Instance.currentTimeState == TimeState.Day)
        {
            // 알파값 증가 또는 감소
            alpha += direction * Time.deltaTime * 0.2f; // 속도 조절 (0.2는 조정 가능)

            // 방향 반전 조건
            if (alpha >= battleScreenMaxAlpha)
            {
                alpha = battleScreenMaxAlpha;
                direction = -1f;
            }
            else if (alpha <= battleScreenMinAlpha)
            {
                alpha = battleScreenMinAlpha;
                direction = 1f;
            }

            battleScreen.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            yield return null; // 매 프레임 갱신
        }

        battleScreen.color = Color.clear;
    }
}
