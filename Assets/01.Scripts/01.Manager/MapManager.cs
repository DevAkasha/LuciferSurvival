using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    protected override bool IsPersistent => false;

    // 맵을 담아두는 리스트, 낮/밤 전환 시 리스트에 있는 모든 맵에 적용
    private List<Map> allMaps = new List<Map>();

    private void Update()
    {
        if (TimeManager.Instance.IsNight()) // 밤일 때
        {
            MapDay();
        }
        else // 낮일 때
        {
            MapNight();
        }
    }

    // 맵-낮
    private void MapDay()
    {
        // 맵에서 낮에 할수있는 효과들 (전투 등)
    }

    // 맵-밤
    private void MapNight()
    {
        // 맵에서 밤에 할수있는 효과들 (파밍 등)
    }
}
