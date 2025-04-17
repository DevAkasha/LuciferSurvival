using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    protected override bool IsPersistent => false;

    // ���� ��Ƶδ� ����Ʈ, ��/�� ��ȯ �� ����Ʈ�� �ִ� ��� �ʿ� ����
    private List<Map> allMaps = new List<Map>();

    private void Update()
    {
        if (TimeManager.Instance.IsNight()) // ���� ��
        {
            MapDay();
        }
        else // ���� ��
        {
            MapNight();
        }
    }

    // ��-��
    private void MapDay()
    {
        // �ʿ��� ���� �Ҽ��ִ� ȿ���� (���� ��)
    }

    // ��-��
    private void MapNight()
    {
        // �ʿ��� �㿡 �Ҽ��ִ� ȿ���� (�Ĺ� ��)
    }
}
