using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileManager : Singleton<TileManager>
{
    protected override bool IsPersistent => false;

    public Transform ResourceTileBlock;     // grid 하위의 오브젝트 (자원타일)
    public Transform lockTileBlock;         // grid 하위의 오브젝트 (잠금타일)

    int gridCount = 15;     // 전체 크기

    [SerializeField] private GameObject lockTileObj;            // 잠금타일 프리펩
    [SerializeField] private List<GameObject> resourceTileObj;  // 자원타일 프리펩 리스트
    [SerializeField] private Grid grid;
    [SerializeField] private List<Vector2Int> excludedCells;    // 제외할 셀 좌표 리스트

    private void Start()
    {
        SetResourceTileMap();
        SetLockTileMap();
    }

    /// <summary>
    /// 자원타일 배치
    /// </summary>
    public void SetResourceTileMap()
    {
        // 그리드 중심으로부터 범위를 계산
        int halfGrid = gridCount / 2;

        for (int x = -halfGrid; x <= halfGrid; x++)
        {
            for (int y = -halfGrid; y <= halfGrid; y++)
            {
                int resourceCount = Random.Range(0, 3); // 0~2사이의 타일이 랜덤으로 생성

                for (int i = 0; i < resourceCount; i++)
                {
                    int resourceType = Random.Range(0, resourceTileObj.Count);

                    Vector3Int gridCell = new Vector3Int(x, y, 0);
                    Vector3 worldPos = grid.CellToWorld(gridCell); // 방의 중심좌표
                    Vector3 addjustedPos = new Vector3(worldPos.x, 0, worldPos.z);

                    Vector3 spawnPos = GetRandomPositionInCell(addjustedPos, 7);

                    Instantiate(resourceTileObj[resourceType], spawnPos, Quaternion.identity, ResourceTileBlock); // 프리팹을 adjustedPos월드위치에 생성
                }
            }
        }
    }

    /// <summary>
    /// 잠금타일
    /// </summary>
    public void SetLockTileMap()
    {
        // 그리드 중심으로부터 범위를 계산
        int halfGrid = gridCount / 2;

        for (int x = -halfGrid; x <= halfGrid; x++)
        {
            for (int y = -halfGrid; y <= halfGrid; y++)
            {
                // 제외할 타일 목록, excludedCells에 있는 좌표도 중심 기준으로 조정해야 할 수 있음
                Vector2Int cell = new Vector2Int(x, y);
                // excludedCells의 좌표 시스템에 맞게 변환 필요
                Vector2Int adjustedCell = new Vector2Int(x + halfGrid, y + halfGrid);

                if (excludedCells.Contains(adjustedCell))
                    continue; // 제외된 영역은 스킵

                Vector3Int gridCell = new Vector3Int(x, y, 0);
                Vector3 worldPos = grid.CellToWorld(gridCell);
                Vector3 addjustedPos = new Vector3(worldPos.x, 0f, worldPos.z);
                Instantiate(lockTileObj, addjustedPos, Quaternion.identity, lockTileBlock); // 프리팹을 addjustedPos월드위치에 생성
            }
        }
    }

    /// <summary>
    /// 일정한 범위 내 랜덤한 위치 생성
    /// </summary>
    /// <param name="center"></param>
    /// <param name="maxOffset"></param>
    /// <returns></returns>
    private Vector3 GetRandomPositionInCell(Vector3 center, float maxOffset)
    {
        // 원형 범위 안 랜덤 위치
        Vector2 offset2D = Random.insideUnitCircle * maxOffset;

        return new Vector3(center.x + offset2D.x, 0, center.z + offset2D.y);
    }


    /// <summary>
    /// 자금타일 위치확인용 기즈모
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        int halfGrid = gridCount / 2;
        Gizmos.color = Color.red; // 기즈모 설정 색
        foreach (var cell in excludedCells)
        {
            Vector2Int adjustedCell = new Vector2Int(cell.x - halfGrid, cell.y - halfGrid);
            Vector3Int gridCell = new Vector3Int(adjustedCell.x, adjustedCell.y, 0); // 3D좌표로 변환
            Vector3 worldPos = grid.CellToWorld(gridCell); // 월드좌표로 변환
            Vector3 pos = new Vector3(worldPos.x, 0f, worldPos.z); // 위치
            Gizmos.DrawWireCube(pos, Vector3.one * 2f); // 와이어프레임 박스 표시 (222 크기의 정육면체)
            Gizmos.DrawWireSphere(pos, 7);
        }
    }
}
