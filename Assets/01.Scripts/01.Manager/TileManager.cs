using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : Singleton<TileManager>
{
    protected override bool IsPersistent => false;

    public GameObject lockBlockTile;

    // 초기 타일 위치 저장용 리스트
    private List<Vector3> OriginalTiles = new List<Vector3>();

    // Vector3Int : 월드 좌표나 그리드 좌표를 기준으로 씀
    // gird position, gridpos : 격자 좌표
    private Dictionary<Vector3Int, GameObject> tiles = new Dictionary<Vector3Int, GameObject>();


    private void Start()
    {
        InitializeTiles();
    }

    // 타일 초기화
    private void InitializeTiles()
    {
        tiles.Clear();
        OriginalTiles.Clear();

        foreach (Transform child in lockBlockTile.transform)
        {
            // 각 타일의 월드 위치를 그리드 좌표로 바꿈
            Vector3Int gridPos = GridPosition(child.position);
            // 그 좌표를 키로 딕셔너리에 저장
            tiles[gridPos] = child.gameObject;

            OriginalTiles.Add(child.position);
        }
    }

    // 상호작용 후 타일 제거, 플레이어용
    public void ClearTile(Vector3 worldPos)
    {
        if (CheckTile(worldPos))
        {
            RemoveTile(worldPos);
        }
    }

    // 그리드 좌표로 변환, 딕셔너리의 키로 만들어줄수있게 하는 함수
    private Vector3Int GridPosition(Vector3 worldPos)
    {
        return Vector3Int.RoundToInt(worldPos);
    }

    // 특정 그리드 위치에 타일이 존재하는지 확인
    public bool CheckTile(Vector3 worldPos)
    {
        return tiles.ContainsKey(GridPosition(worldPos));
    }

    // 상호작용한 위치의 타일 제거
    public void RemoveTile(Vector3 worldPos)
    {
        Vector3Int gridPos = GridPosition(worldPos);

        if (tiles.TryGetValue(gridPos, out GameObject tile))
        {
            Destroy(tile);
            tiles.Remove(gridPos);
        }
    }

    public void ResetTiles()
    {
        // 기존 타일 삭제
        foreach (var tile in tiles.Values)
        {
            if (tile != null)
                Destroy(tile);
        }

        tiles.Clear();

        // 초기 위치 기반으로 새로 생성
        foreach (Vector3 pos in OriginalTiles)
        {
            GameObject newTile = Instantiate(lockBlockTile, pos, Quaternion.identity, lockBlockTile.transform);
            Vector3Int gridPos = GridPosition(pos);
            tiles[gridPos] = newTile;
        }
    }
}
