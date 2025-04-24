using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : Singleton<TileManager>
{
    protected override bool IsPersistent => false;

    public GameObject lockBlockTile; // 잠금타일
    public GameObject resourceTile; // 자원타일

    int gridCount = 20;

    [SerializeField] private GameObject lockTileObj;
    [SerializeField] private Grid grid;
    [SerializeField] private List<Vector2Int> excludedCells; // 제외할 셀 좌표들

    private void Start()
    {
        SetResourceTileMap();
        SetLockTileMap();
    }

    public void SetResourceTileMap()
    {

    }

    // 잠금타일 배치 메서드
    public void SetLockTileMap()
    {
        // 프리펩 크기 조절할수있게 추가 하나 해줘보기, 공부하기
        // 현재 포지션 000기준으로 생성되는 형식인데 중앙에서 퍼져나가는 형식으로도 가능한지 나중에 확인해보기

        for (int x = 0; x < gridCount; x++)
        {
            for (int y = 0; y < gridCount; y++)
            {
                Vector2Int cell = new Vector2Int(x, y); // 2D좌표로 만듬, 제외시킬 친구들
                if (excludedCells.Contains(cell))
                    continue; // 제외된 영역은 스킵
                Vector3Int gridCell = new Vector3Int(x, y, 0); // grid 좌표로 변환
                Vector3 worldPos = grid.CellToWorld(gridCell); // gridcell 위치를 월드좌표로 변환, 정확한 위치 파악, XZ 평면 → Y를 0으로 고정
                Vector3 addjustedPos = new Vector3(worldPos.x, 0f, worldPos.z); // y축은 0으로 고정, xz평면위에 배치, adjustedPos:최종위치값
                Instantiate(lockTileObj, addjustedPos, Quaternion.identity, transform); // lockTile프리팹을 adjustedPos월드위치에 생성, 회전없이, 부모는 현재객체
            }
        }
    }

    // 잠금타일 제외시킬 위치를 확인하기 위해 필요한 기즈모 (위치 시각화 용도)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach (var cell in excludedCells)
        {
            Vector3Int gridCell = new Vector3Int(cell.x, cell.y, 0);
            Vector3 worldPos = grid.CellToWorld(gridCell);
            Vector3 pos = new Vector3(worldPos.x, 0f, worldPos.z);
            Gizmos.DrawWireCube(pos, Vector3.one * 2f);
        }
    }
}
