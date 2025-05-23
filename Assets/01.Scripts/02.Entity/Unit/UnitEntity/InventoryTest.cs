using UnityEngine;

public class InventoryTest : MonoBehaviour
{
    private UnitModel[] testUnits;

    private void Start()
    {
        // 테스트용 UnitModel 생성
        testUnits = new UnitModel[3];

        for (int i = 0; i < testUnits.Length; i++)
        {
            testUnits[i] = new UnitModel(DataManager.Instance.GetData<UnitDataSO>("UNIT0001"));
            testUnits[i].rcode = $"unit_{i}";
            testUnits[i].displayName = $"유닛_{i}";
        }

        UnitManager.Instance.AddUnit(testUnits[0]);
        UnitManager.Instance.AddUnit(testUnits[0]);
        UnitManager.Instance.AddUnit(testUnits[0]);
        UnitManager.Instance.AddUnit(testUnits[0]);
        UnitManager.Instance.AddUnit(testUnits[0]);
        UnitManager.Instance.PrintInventory();
    }
}