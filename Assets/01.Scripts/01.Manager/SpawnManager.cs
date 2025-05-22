using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class SpawnManager : Singleton<SpawnManager>
{
    [SerializeField] Camera mainCamera;
    [SerializeField] Transform target;

    public float SpawnRange = 30f;

    void Start()
    {
        target = PlayerManager.Instance.Player.transform;
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    public void EnemySpawn(string rcode)
    {
        var enemy = PoolManager.Instance.Spawn<ObjectPoolBase>(rcode, SpawnArea());
    }

    Vector3 SpawnArea()
    {
        if (mainCamera == null)
        {
            Debug.Log("카메라 없음");
            return Vector3.zero;
        }
        if (target == null)
        {
            Debug.Log("플레이어 없음");
            return Vector3.zero;
        }

        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(mainCamera);//카메라가 보이는 영역을 평면화

        for (int i = 0; i < 300; i++)
        {
            Vector2 randomXZ = Random.insideUnitCircle.normalized * Random.Range(SpawnRange * 0.5f, SpawnRange);//xz 평면 상의 원 그리기
            Vector3 spawnCircle = new Vector3(randomXZ.x, 0f, randomXZ.y) + new Vector3(target.position.x, 0f, target.position.z);//원의 중심을 정하고 범위 지정

            // NavMesh 위 확인
            if (NavMesh.SamplePosition(spawnCircle, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                Vector3 navPos = hit.position;

                //카메라 시야 밖 여부 확인
                Bounds bounds = new Bounds(navPos + Vector3.up * 1f, Vector3.one);
                if (!GeometryUtility.TestPlanesAABB(frustumPlanes, bounds))
                {
                    //Debug.Log($"{navPos}");
                    return navPos;
                }
            }
        }

        Debug.Log("먼가 없음");
        return new Vector3(50, 0, 50);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SpawnManager))]
public class EnemySpawn : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (EditorApplication.isPlaying)
        {
            if (GUILayout.Button("적 생성"))
            {
                ((SpawnManager)target).EnemySpawn("ENE0001");
            }
        }
    }
}
#endif