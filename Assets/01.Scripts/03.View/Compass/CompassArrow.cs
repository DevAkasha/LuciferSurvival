using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassArrow : MonoBehaviour
{
    [SerializeField] private Transform playerTransform; // 플레이어 위치
    [SerializeField] private Transform targetTransform; // 추적할 타겟
    [SerializeField] private RectTransform arrowImage; // 화살표 이미지

    private void Update()
    {
        if (playerTransform == null || targetTransform == null || arrowImage == null)
            return;

        UpdateCompassArrow();
    }

    private void UpdateCompassArrow()
    {
        // 플레이어에서 타겟으로의 방향 벡터 계산 (y축 무시)
        Vector3 targetDirection = targetTransform.position - playerTransform.position;
        targetDirection.y = 0f;

        // 타겟 방향을 각도로 변환
        float angle = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;

        // 화살표 이미지 회전
        arrowImage.localRotation = Quaternion.Euler(0, 0, -angle);
    }

    // 타겟 변경 메서드
    public void SetTarget(Transform newTarget)
    {
        targetTransform = newTarget;
    }

    // 플레이어 변경 메서드
    public void SetPlayer(Transform newPlayer)
    {
        playerTransform = newPlayer;
    }
}