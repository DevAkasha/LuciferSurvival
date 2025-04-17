using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SummonUnitUI : MonoBehaviour
{
    [SerializeField]
    private Transform summonSlotLayout;

    [SerializeField]
    private SummonSlot summonSlotPrefab;

    [SerializeField]
    private List<Sprite> gradeSprites;

    private int shopLevel = 1;

    private void Start()
    {
        SetRandomUnit();
    }

    public void OnclickShopLevelUp()
    {
        //���� ���� ���� üũ �� �������� ������ ��ȭ�̸� ������
    }

    public void OnclickRerollUnit()
    {
        SummonTableUtil.ClearAllChildren(summonSlotLayout);
        SetRandomUnit();
    }

    private void SetRandomUnit()
    {
        List<UnitDataSO> units = SummonTableUtil.RerollShop(shopLevel);

        foreach (UnitDataSO unitData in units)
        {
            SummonSlot slot = Instantiate(summonSlotPrefab, summonSlotLayout);
            slot.SetSlot(unitData);
        }
    }
}
