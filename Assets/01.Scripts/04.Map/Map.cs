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

    // 밤엔 파밍이 가능해지게
    public void SetNightMode(bool night)
    {
        isNight = night;
        if (isNight)
        {
            // 파밍 가능해지게 하는 메서드 혹은 코드 추가
        }
    }
}
