using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    protected override bool IsPersistent => false;

    // ���� ��Ƶδ� ����Ʈ, ��/�� ��ȯ �� ����Ʈ�� �ִ� ��� �ʿ� ����
    private List<Map> allMaps = new List<Map>();

    // ������Ʈ�� Ȱ��ȭ�ɶ�
    private void OnEnable()
    {
        TimeManager.Instance.OnDay += StartDay;
        TimeManager.Instance.OnNight += StartNight;
    }

    // ������Ʈ�� ��Ȱ��ȭ�ɶ�, �޸� ������ ���ؼ� ������ �̺�Ʈ ����
    private void OnDisable()
    {
        TimeManager.Instance.OnDay -= StartDay;
        TimeManager.Instance.OnNight -= StartNight;
    }

    // �ܺο��� ���� ���, �ߺ��� ���� ���� Contains üũ
    public void RegisterMap(Map map)
    {
        if (!allMaps.Contains(map))
            allMaps.Add(map);
    }

    // ������ ��ȯ
    private void StartDay()
    {
        foreach (var map in allMaps)
        {
            map.SetNightMode(false);
        }
    }

    // ������ ��ȯ
    private void StartNight()
    {
        foreach (var map in allMaps)
        {
            map.SetNightMode(true);
        }
    }
}
