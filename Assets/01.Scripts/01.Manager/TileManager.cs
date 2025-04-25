using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileManager : Singleton<TileManager>
{
    protected override bool IsPersistent => false;

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
        for (int x = 0; x < gridCount; x++)
        {
            for (int y = 0; y < gridCount; y++)
            {
                int resourceCount = Random.Range(0, 3); // 0~2사이의 타일이 랜덤으로 생성

                for (int i = 0; i < resourceCount; i++)
                {
                    int resourceType = Random.Range(0, resourceTileObj.Count);

                    Vector3Int gridCell = new Vector3Int(x, y, 0);
                    Vector3 worldPos = grid.CellToWorld(gridCell); // 방의 중심좌표
                    Vector3 addjustedPos = new Vector3(worldPos.x, 0, worldPos.z);

                    Vector3 spawnPos = GetRandomPositionInCell(addjustedPos, 7);

                    Instantiate(resourceTileObj[resourceType], spawnPos, Quaternion.identity, transform); // 프리팹을 adjustedPos월드위치에 생성
                }
            }
        }

        //// 자원타일이 이미 배치된 좌표 확인용
        //List<Vector2Int> useTileObj = new List<Vector2Int>();


        //for (int i = 0; i < resourceCount; i++)
        //{
        //int randomX = Random.Range(0, gridCount);
        //int randomY = Random.Range(0, gridCount);
        //    int resourceType = Random.Range(0, resourceTileObj.Count);

        //    Vector2Int cell = new Vector2Int(randomX, randomY); // 랜덤 좌표 생성
        //    if (useTileObj.Contains(cell)) // 이미 배치된 좌표일경우 반복횟수 복원
        //    {
        //        i--;
        //        continue;
        //    }
        //    useTileObj.Add(cell); // 좌표 등록

        //    Vector3Int gridCell = new Vector3Int(randomX, randomY, 0); // grid 좌표로 변환
        //    Vector3 worldPos = grid.CellToWorld(gridCell); // 월드좌표로 변환
        //    Vector3 addjustedPos = new Vector3(worldPos.x, 0f, worldPos.z); // 최종위치값 = y축은 0으로 고정, xz평면위에 배치
        //    Instantiate(resourceTileObj[resourceType], addjustedPos, Quaternion.identity, transform); // 프리팹들을 adjustedPos월드위치에 생성
        //}
    }

    /// <summary>
    /// 잠금타일
    /// </summary>
    public void SetLockTileMap()
    {
        for (int x = 0; x < gridCount; x++)
        {
            for (int y = 0; y < gridCount; y++)
            {
                Vector2Int cell = new Vector2Int(x, y); // 제외할 타일 목록
                if (excludedCells.Contains(cell))
                    continue; // 제외된 영역은 스킵
                Vector3Int gridCell = new Vector3Int(x, y, 0);
                Vector3 worldPos = grid.CellToWorld(gridCell);
                Vector3 addjustedPos = new Vector3(worldPos.x, 0f, worldPos.z);
                Instantiate(lockTileObj, addjustedPos, Quaternion.identity, transform); // 프리팹을 adjustedPos월드위치에 생성
            }
        }
    }

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
        Gizmos.color = Color.red; // 기즈모 설정 색
        foreach (var cell in excludedCells)
        {
            Vector3Int gridCell = new Vector3Int(cell.x, cell.y, 0); // 3D좌표로 변환
            Vector3 worldPos = grid.CellToWorld(gridCell); // 월드좌표로 변환
            Vector3 pos = new Vector3(worldPos.x, 0f, worldPos.z); // 위치
            Gizmos.DrawWireCube(pos, Vector3.one * 2f); // 와이어프레임 박스 표시 (222 크기의 정육면체)
            Gizmos.DrawWireSphere(pos, 7);
        }
    }
}
