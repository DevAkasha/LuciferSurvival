using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;

    [SerializeField] private Vector3 offset = new Vector3(0f, 10f, -7.1f);

    // 카메라 고정 회전 (탑다운 뷰)
    [SerializeField] private Vector3 fixedRotation = new Vector3(60f, 0f, 0f);

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
        }

        // 초기 위치 설정
        if (target != null)
        {
            transform.position = target.position + offset;
        }

        // 카메라 회전 고정 (플레이어 회전과 무관하게)
        transform.rotation = Quaternion.Euler(fixedRotation);
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

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