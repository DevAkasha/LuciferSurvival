using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MapManager
{
    private bool isNight;

    private void Start()
    {
        RegisterMap(this);
    }

    public void SetNightMode(bool night)
    {
        isNight = night;
        if (isNight)
        {
            // �Ĺ� ����������
        }
    }
}
