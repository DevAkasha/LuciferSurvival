using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    protected override bool IsPersistent => false;

    // 맵을 담아두는 리스트, 낮/밤 전환 시 리스트에 있는 모든 맵에 적용
    private List<Map> allMaps = new List<Map>();

    // 오브젝트가 활성화될때
    private void OnEnable()
    {
        TimeManager.Instance.OnDay += StartDay;
        TimeManager.Instance.OnNight += StartNight;
    }

    // 오브젝트가 비활성화될때, 메모리 누수를 위해서 꺼질때 이벤트 제거
    private void OnDisable()
    {
        TimeManager.Instance.OnDay -= StartDay;
        TimeManager.Instance.OnNight -= StartNight;
    }

    // 외부에서 맵을 등록, 중복을 막기 위해 Contains 체크
    public void RegisterMap(Map map)
    {
        if (!allMaps.Contains(map))
            allMaps.Add(map);
    }

    // 낮으로 전환
    private void StartDay()
    {
        foreach (var map in allMaps)
        {
            map.SetNightMode(false);
        }
    }

    // 밤으로 전환
    private void StartNight()
    {
        foreach (var map in allMaps)
        {
            map.SetNightMode(true);
        }
    }
}
