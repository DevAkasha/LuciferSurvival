using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : Singleton<TileManager>
{
    protected override bool IsPersistent => false;

    public GameObject lockBlockTile; // 잠금타일
    public GameObject resourceTile; // 자원타일

    int gridCount = 100;

    [SerializeField] private GameObject lockTileObj;
    [SerializeField] private Grid grid;
    [SerializeField] private List<Vector2Int> excludedCells; // 제외할 셀 좌표들

    private void Start()
    {
        SetLockTileMap();
    }

    public void SetLockTileMap()
    {
        // 프리펩 크기 조절할수있게 추가 하나 해줘보기, 공부하기

        for (int x = 0; x < gridCount; x++)
        {
            for (int y = 0; y < gridCount; y++)
            {
                Vector2Int cell = new Vector2Int(x, y);
                //if (excludedCells.Contains(cell))
                //  continue; // 제외된 영역은 스킵
                Vector3Int gridCell = new Vector3Int(x, y, 0);
                Vector3 worldPos = grid.CellToWorld(gridCell);
                // XZ 평면 → Y를 0으로 고정
                Vector3 adjustedPos = new Vector3(worldPos.x, 0f, worldPos.z);
                Instantiate(lockTileObj, adjustedPos, Quaternion.identity, transform);
            }
        }
    }
}
