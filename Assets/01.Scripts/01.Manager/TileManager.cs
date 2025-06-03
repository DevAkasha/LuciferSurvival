using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileManager : Singleton<TileManager>
{
    protected override bool IsPersistent => false;

    public Transform ResourceTileBlock;     // grid 하위의 오브젝트 (자원타일)
    public Transform lockTileBlock;         // grid 하위의 오브젝트 (잠금타일)

    int gridCount = 36;     // 전체 크기
    public int EnviromentCode;

    [SerializeField] private GameObject lockTileObj;            // 잠금타일 프리펩
    [SerializeField] private List<GameObject> resourceTileObj;  // 자원타일 프리펩 리스트
    [SerializeField] private Grid grid;
    [SerializeField] private List<Vector2Int> excludedCells;    // 제외할 셀 좌표 리스트
    [SerializeField] private List<GameObject> environmentTemplate; // 추후 사용, 환경 등 변화에 대한 리스트

    [SerializeField] private float hexagonRadius = 5f;          // 6각형 타일의 반지름
    [SerializeField] private float resourceMinDistance = 6.0f;  // 자원 간 최소 거리

    private Dictionary<Vector2Int, List<Vector3>> cellResources = new Dictionary<Vector2Int, List<Vector3>>();

    [SerializeField] private GameObject soulAltarPrefab; // 영혼의제단
    [SerializeField] private CompassArrow compassArrow; // 나침반

    private void Start()
    {
        SetResourceTileMap();
        SetLockTileMap();
        SetSoulAltar();
    }

    public void SetEnviroment()
    {
        if (environmentTemplate == null)
            return;
        if(EnviromentCode < 0 && EnviromentCode > environmentTemplate.Count)
            return;

        Instantiate(resourceTileObj[EnviromentCode], Vector3.zero, Quaternion.identity);
    }

    // 자원타일 배치
    public void SetResourceTileMap()
    {
        // 그리드 중심으로부터 범위를 계산
        int halfGrid = gridCount / 2;

        // 이미 사용된 위치를 추적하는 리스트
        List<Vector3> usedPositions = new List<Vector3>();

        for (int x = -halfGrid; x <= halfGrid; x++)
        {
            for (int y = -halfGrid; y <= halfGrid; y++)
            {
                // 중앙 타일(0,0)은 건너뜀
                if (x == 0 && y == 0)
                    continue;

                int resourceCount = Random.Range(0, 3); // 0~2사이의 타일이 랜덤으로 생성
                Vector3Int gridCell = new Vector3Int(x, y, 0);
                Vector3 worldPos = grid.CellToWorld(gridCell);
                Vector3 centerPos = new Vector3(worldPos.x, 0, worldPos.z);

                for (int i = 0; i < resourceCount; i++)
                {
                    // 겹치지 않는 위치 찾기
                    Vector3 spawnPos;
                    bool validPosition = false;

                    // 최대 5번 시도
                    for (int attempt = 0; attempt < 5; attempt++)
                    {
                        spawnPos = GetRandomPositionInCell(centerPos, hexagonRadius);

                        // 다른 자원과 겹치는지 확인
                        validPosition = true;
                        foreach (var pos in usedPositions)
                        {
                            if (Vector3.Distance(spawnPos, pos) < resourceMinDistance)
                            {
                                validPosition = false;
                                break;
                            }
                        }

                        // 겹치지 않는 위치를 찾았으면 자원 생성
                        if (validPosition)
                        {
                            int resourceType = Random.Range(0, resourceTileObj.Count);
                            Instantiate(resourceTileObj[resourceType], spawnPos, Quaternion.identity, ResourceTileBlock);
                            usedPositions.Add(spawnPos);
                            break;
                        }
                    }
                }
            }
        }
    }

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

    // 일정한 범위 내 랜덤한 위치 생성
    private Vector3 GetRandomPositionInCell(Vector3 center, float maxOffset)
    {
        // 원형 범위 안 랜덤 위치
        Vector2 offset2D = Random.insideUnitCircle * maxOffset;

        return new Vector3(center.x + offset2D.x, 0, center.z + offset2D.y);
    }

    // 영혼의 제단 소환
    public void SetSoulAltar()
    {
        int halfGrid = gridCount / 2;

        Vector2Int centerCell = new Vector2Int(halfGrid, halfGrid);
        Vector3Int gridCell = new Vector3Int(0, 0, 0);
        Vector3 worldPos = grid.CellToWorld(gridCell);
        Vector3 altarPos = new Vector3(worldPos.x, 0f, worldPos.z + -1.5f);

        if (soulAltarPrefab != null)
        {
            GameObject go = Instantiate(soulAltarPrefab, altarPos, Quaternion.identity, ResourceTileBlock);
            compassArrow.SetTarget(go.transform);
        }
    }

    // 위치확인용 기즈모
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
