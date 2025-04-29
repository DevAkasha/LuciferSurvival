using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    protected override bool IsPersistent => false;

    // 맵 동적 생성 - 만들어진 지형 프리펩을 랜덤으로 가져오기
    // 타일매니저에 있는것 응용하여 프리팹을 랜덤으로 하나 가져와 배치?
    // 테마에 맞게 맵이 선정되어야 하는것이니 기획자분의 의도에 맞게 랜덤이 아닌 시작할때 지정해주는걸로 진행할수도 있음
}
