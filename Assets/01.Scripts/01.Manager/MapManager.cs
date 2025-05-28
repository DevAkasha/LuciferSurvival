using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    protected override bool IsPersistent => false;

    [Header("Map Settings")]
    public Transform BaseMapBlock;
    [SerializeField] private List<GameObject> mapPrefabs;

    [Header("Wall Settings")]
    public GameObject wallPrefab;           // 벽 프리팹
    public Transform wallParent;            // 벽들의 부모 오브젝트
    public float wallSpacing = 2f;          // 벽 프리팹 간의 간격
    public float marginFromMap = 1f;        // 맵으로부터의 여백

    private GameObject selectedMapPrefab;
    private GameObject currentMap;
    private List<GameObject> generatedWalls = new List<GameObject>();

    private void Start()
    {
        CreateMap();
    }

    public void CreateMap()
    {
        ClearMap();

        int map = Random.Range(0, mapPrefabs.Count);
        selectedMapPrefab = mapPrefabs[map];
        currentMap = Instantiate(selectedMapPrefab, Vector3.zero, Quaternion.identity, BaseMapBlock);

        if (wallPrefab != null)
        {
            CreateWalls();
        }
    }

    private void CreateWalls()
    {
        Bounds mapBounds = GetCombinedBounds(currentMap);
        Transform parent = wallParent != null ? wallParent : BaseMapBlock;

        // 벽 프리팹의 실제 크기 계산
        Bounds wallBounds = GetPrefabBounds(wallPrefab);
        float wallWidth = wallBounds.size.x;
        float wallDepth = wallBounds.size.z;
        float wallHeight = wallBounds.size.y;

        Vector3 center = mapBounds.center;
        Vector3 size = mapBounds.size;

        // 벽을 더 안쪽으로 배치 (marginFromMap 값을 더 크게)
        float halfWidth = (size.x / 2f) - marginFromMap - (wallDepth / 2f);
        float halfDepth = (size.z / 2f) - marginFromMap - (wallDepth / 2f);

        // 맵 위에 올라오도록 Y 위치 조정
        float wallY = mapBounds.max.y + (wallHeight / 2f); // 맵 최고점 + 벽 높이의 절반

        // 북쪽 벽 (Z+)
        CreateWallLine(
            new Vector3(center.x - halfWidth, wallY, center.z + halfDepth),
            Vector3.right,
            halfWidth * 2,
            wallWidth,
            0f,
            parent
        );

        // 남쪽 벽 (Z-)
        CreateWallLine(
            new Vector3(center.x - halfWidth, wallY, center.z - halfDepth),
            Vector3.right,
            halfWidth * 2,
            wallWidth,
            0f,
            parent
        );

        // 동쪽 벽 (X+)
        CreateWallLine(
            new Vector3(center.x + halfWidth, wallY, center.z - halfDepth + wallWidth),
            Vector3.forward,
            (halfDepth * 2) - (wallWidth * 2),
            wallDepth,
            90f,
            parent
        );

        // 서쪽 벽 (X-)
        CreateWallLine(
            new Vector3(center.x - halfWidth, wallY, center.z - halfDepth + wallWidth),
            Vector3.forward,
            (halfDepth * 2) - (wallWidth * 2),
            wallDepth,
            90f,
            parent
        );
    }

    private void CreateWallLine(Vector3 startPos, Vector3 direction, float totalLength, float segmentSize, float rotationY, Transform parent)
    {
        // 겹치지 않도록 벽 개수 계산
        int wallCount = Mathf.FloorToInt(totalLength / segmentSize);

        // 실제 사용할 간격 계산 (약간의 여백 포함)
        float actualSpacing = segmentSize * 0.95f; // 5% 여백으로 겹침 방지

        // 시작 위치 조정 (중앙 정렬)
        float totalUsedLength = wallCount * actualSpacing;
        Vector3 adjustedStartPos = startPos + direction * ((totalLength - totalUsedLength) / 2f);

        for (int i = 0; i < wallCount; i++)
        {
            Vector3 position = adjustedStartPos + direction * (actualSpacing * i) + direction * (segmentSize / 2f);
            Quaternion rotation = Quaternion.Euler(0, rotationY, 0);

            GameObject wall = Instantiate(wallPrefab, position, rotation, parent);
            generatedWalls.Add(wall);
        }
    }


    private Bounds GetPrefabBounds(GameObject prefab)
    {
        // 프리팹의 실제 크기를 얻기 위해 임시로 생성
        GameObject temp = Instantiate(prefab);
        Bounds bounds = GetCombinedBounds(temp);
        Destroy(temp);
        return bounds;
    }

    private Bounds GetCombinedBounds(GameObject obj)
    {
        Bounds bounds = new Bounds(obj.transform.position, Vector3.zero);
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        if (renderers.Length > 0)
        {
            foreach (Renderer r in renderers)
            {
                bounds.Encapsulate(r.bounds);
            }
        }
        else
        {
            Collider[] colliders = obj.GetComponentsInChildren<Collider>();
            foreach (Collider c in colliders)
            {
                bounds.Encapsulate(c.bounds);
            }
        }

        return bounds;
    }

    private void ClearMap()
    {
        if (currentMap != null)
        {
            Destroy(currentMap);
        }

        foreach (GameObject wall in generatedWalls)
        {
            if (wall != null)
                Destroy(wall);
        }
        generatedWalls.Clear();
    }

    public void GenerateNewMap()
    {
        CreateMap();
    }

    //public Transform BaseMapBlock;     // 베이스 맵을 담을 부모 Transform
    //[SerializeField] private List<GameObject> mapPrefabs;  // 맵 프리펩 리스트

    //private GameObject selectedMapPrefab; // 선택된 맵 프리펩
    //private GameObject currentMap;      // 현재 생성된 맵 게임오브젝트의 참조

    //private void Start()
    //{
    //    CreateMap();
    //}

    //// 선택된 프리펩으로 맵 생성
    //public void CreateMap()
    //{
    //    // 랜덤 인덱스 생성
    //    int map = Random.Range(0, mapPrefabs.Count);

    //    // 랜덤 맵 프리팹 선택
    //    selectedMapPrefab = mapPrefabs[map];

    //    // 맵 프리팹 생성 - 중앙 위치에 배치
    //    currentMap = Instantiate(selectedMapPrefab, Vector3.zero, Quaternion.identity, BaseMapBlock);
    //}
}
