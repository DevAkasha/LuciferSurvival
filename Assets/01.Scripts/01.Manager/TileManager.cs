using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : Singleton<TileManager>
{
    protected override bool IsPersistent => false;

    public GameObject lockBlockTile;

    // Vector3Int : 월드 좌표나 그리드 좌표를 기준으로 씀 - gird position, gridpos : 격자 좌표, 딕셔너리에 타일 오브젝트 저장
    public Dictionary<Vector3Int, GameObject> tiles = new Dictionary<Vector3Int, GameObject>();

    // 그리드 좌표로 변환, 딕셔너리의 키로 만들어줄수있게 하는 함수, 플레이어쪽에선 그리드 좌표를 구하고 타일이 있는지 확인 후 상호작용으로 처리 가능
    private Vector3Int GridPosition(Vector3 worldPos)
    {
        return Vector3Int.RoundToInt(worldPos);
    }

    public void RemoveTile()
    {
        // 두 메서드가 매니저에선 가능한데 플레이어에선 어려울듯함, 타일을 플레이어가 이미 알고있기때문에 호출을해줄거라 아무런 매개변수를 받지않는 리무브타일을 만들면된다.
        // 이거는 오버로드로 하면 된다, 리무브타일()이 오버로드하는거
        // 플레이어가 파괴해야하는 것에 대해 넣기,

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
}
