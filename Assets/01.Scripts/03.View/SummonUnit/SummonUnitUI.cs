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
        //���� ���� ���� üũ �� �������� ������ ��ȭ�̸� ������
    }

    public void SetUnitTable(List<UnitDataSO> unitDatas)
    {
        
    }

    public void OnclickRerollUnit()
    {

    }
}
