using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;

    [SerializeField] private Vector3 offset = new Vector3(0f, 10f, -7.1f);

    // 카메라 고정 회전 (탑다운 뷰)
    [SerializeField] private Vector3 fixedRotation = new Vector3(60f, 0f, 0f);

    // 카메라 줌 인 세팅
    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 2f; // 줌 속도
    [SerializeField] private float minZoom = 8f; // 최소 줌 거리
    [SerializeField] private float maxZoom = 25f; // 최대 줌 거리

    private void Start()
    {
        target = PlayerManager.Instance.Player.transform;
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                Debug.Log("카메라 타겟이 자동으로 Player 태그 오브젝트로 설정되었습니다.");
            }
            else
            {
                Debug.LogWarning("카메라 타겟이 설정되지 않았습니다! Player 태그를 가진 오브젝트가 없습니다.");
            }

            transform.position = target.position + offset;
        }

        transform.rotation = Quaternion.Euler(fixedRotation); // 카메라 회전 고정
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        float distance = offset.magnitude; // 현재 줌 거리 계산 (offset 길이)

        // 마우스 휠 줌 인/아웃 처리
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            distance -= scroll * zoomSpeed;
            distance = Mathf.Clamp(distance, minZoom, maxZoom);

            // 방향은 유지하고, 거리만 변경
            offset = offset.normalized * distance;
        }

        transform.position = target.position + offset;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }
}