using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    protected override bool IsPersistent => false;

    public Transform BaseMapBlock;     // 베이스 맵을 담을 부모 Transform
    [SerializeField] private List<GameObject> mapPrefabs;  // 맵 프리펩 리스트

    private GameObject selectedMapPrefab; // 선택된 맵 프리펩
    private GameObject currentMap;      // 현재 생성된 맵 게임오브젝트의 참조

    private void Start()
    {
        SelectMap();
        CreateMap();
    }

    // 맵 프리펩 선택 메서드 
    public void SelectMap()
    {
        // 랜덤 인덱스 생성
        int map = Random.Range(0, mapPrefabs.Count);

        // 랜덤 맵 프리팹 선택
        selectedMapPrefab = mapPrefabs[map];
    }

    // 선택된 프리펩으로 맵 생성
    public void CreateMap()
    {
        // 맵 프리팹 생성 - 중앙 위치에 배치
        currentMap = Instantiate(selectedMapPrefab, Vector3.zero, Quaternion.identity, BaseMapBlock);
    }
}
