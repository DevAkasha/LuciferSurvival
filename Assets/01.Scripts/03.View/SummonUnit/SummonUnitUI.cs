using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SummonUnitUI : MonoBehaviour
{
    [SerializeField]
    private Transform SummonSlotLayout;

    private List<UnitDataSO> unitDatas;

    [SerializeField]
    private List<Sprite> gradeSprites;

    

    private void Start()
    {
        //unitDatas = DataManager.instance.GetDatas<UnitDataSO>();
    }

    public void OnclickShopLevelUp()
    {
        //현재 상점 레벨 체크 후 레벨업이 가능한 재화이면 레벨업
    }

    public void SetUnitTable(List<UnitDataSO> unitDatas)
    {
        
    }

    public void OnclickRerollUnit()
    {

    }
}
