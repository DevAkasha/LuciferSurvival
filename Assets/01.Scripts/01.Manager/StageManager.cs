using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEditor;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    public List<StageModel> allStageList = new(); //@sm GameStageList 모든 스테이지 정보
    public StageModel thisStage;//@sm
    public int stageNumber;//@sm
    public int waveRound;//@sm

    public RxVar<int> soulStone = new RxVar<int>(0); //@SM           //게임 내 재화(초기값 : 0)

    private void Start()
    {
        Init();
        SetStage(0);//테스트 용
    }

    protected override void Awake()
    {
        base.Awake();
        SoulStone = 9; //테스트 코드인가요?
    }

    public int SoulStone 
    {
        get { return soulStone.Value; }
        set { soulStone.SetValue(value, this); }
    }

    public bool ReduceSoulStone(int amount)
    {
        if (soulStone.Value >= amount)
        {
            SoulStone -= amount;
            return true;
        }
        return false;
    }
    public void SetStage(int stageIndex)//@sm
    {
        if (0 > stageIndex || stageIndex > allStageList.Count)
            return;

        waveRound = 0;
        thisStage = allStageList[stageIndex];
        Debug.Log($"스테이지{stageIndex + 1} 준비");
    }

    public void ChangeToNight()//@sm 밤으로 전환, 다음 라운드로 Data 변경
    {
        if (thisStage == null)
            return;

        if (waveRound < 0 || waveRound > thisStage.StageData.Count)
            waveRound = 0;

        WaveManager.Instance.SetWave(thisStage.StageData[waveRound]);
        TimeManager.Instance.SetNight();
    }

    public void ChangeToDay()//@sm 낮으로 전환. 웨이브 시작
    {
        Debug.Log($"{waveRound + 1}웨이브 시작");
        TimeManager.Instance.SetDay();
        WaveManager.Instance.GenerateWave();

        waveRound++;
    }

    public bool UnequipUnit(int equipIndex)
    {
        if (equipIndex < 0 || equipIndex >= equippedUnits.Length)
            return false;

        UnitModel unit = equippedUnits[equipIndex];
        if (unit == null)
            return false;

        AddUnit(unit);

        PlayerManager.Instance.Player.RemoveUnitTransform(equipIndex);

        equippedUnits[equipIndex] = null;

        StageUIManager.Instance.RefreshEquipSlot(equipIndex);
        StageUIManager.Instance.RefreshAllUnitSlots();

        return true;
    }

    public void OnWaveEnd()//@sm
    {
        if (waveRound < thisStage.StageData.Count)
        {
            ChangeToNight();
            Debug.Log("다음 웨이브 준비");
        }
        else
        {
            StageUIManager.Instance.OnStageCleatWindow();
            Debug.Log("스테이지 클리어!");
        }
    }

    public void Init() //@sm
    {
        for (int i = 1; i < 8; i++)
        {
            StageDataSO StageData = DataManager.Instance.GetData<StageDataSO>($"STG000{i}");

            if (StageData == null)
            {
                continue;
            }
            allStageList.Add(new StageModel(StageData));
        }
    }
    
    public bool CheckUnit(string rcode, int count = 1)
    {
        int findCount = 0;

        for (int i = 0; i < unitSlots.Length; i++)
        {
            if (unitSlots[i] != null && unitSlots[i].unitModel.rcode.Equals(rcode))
            {
                findCount++;
            }

            if (findCount >= count)
            {
                return true;
            }
        }

        for (int i = 0; i < equippedUnits.Length; i++)
        {
            if (equippedUnits[i] != null && equippedUnits[i].rcode.Equals(rcode))
            {
                findCount++;
            }

            if (findCount >= count)
            {
                return true;
            }
        }
        return false;
    }

    public int CalculateCompletionRate(UnionTableSO table)
    {
        //Linq를 사용하여 unit4의 값이 없으면 unit4를 제거
        List<string> codes = new List<string> { table.unit1, table.unit2, table.unit3, table.unit4 }
                              .Where(code => !string.IsNullOrEmpty(code))
                              .ToList();

        int totalGrade = 0;
        int ownedGrade = 0;

        foreach (string code in codes)
        {
            UnitDataSO data = DataManager.Instance.GetData<UnitDataSO>(code);
            int grade = (int)data.grade;  // 예: UnitDataSO.grade
            totalGrade += grade;

            if (CheckUnit(code))
                ownedGrade += grade;
        }

        if (totalGrade <= 0f)
            return 0;

        float rawRate = (float)ownedGrade / totalGrade * 100f;
        return Mathf.RoundToInt(rawRate);
    }

    // 현재 인벤토리에서 존재하는 유닛의 수를 확인하는 함수
    public int GetUnitCount(string rcode)
    {
        int total = 0;
        foreach (StackableUnitModel slot in unitSlots)
        {
            if (slot != null && slot.unitModel.rcode == rcode)
                total += slot.count;
        }

        foreach (UnitModel slot in equippedUnits)
        {
            if (slot != null && slot.rcode == rcode)
                total++;
        }
        return total;
    }

    // 인벤토리에서 rcode 유닛을 count만큼 소비
    private bool ConsumeUnit(string rcode, int count)
    {
        int remaining = count;
        for (int i = 0; i < unitSlots.Length && remaining > 0; i++)
        {
            var slot = unitSlots[i];
            if (slot != null && slot.unitModel.rcode == rcode)
            {
                int remove = Mathf.Min(slot.count, remaining);
                for (int k = 0; k < remove; k++)
                    RemoveUnit(i);
                remaining -= remove;
            }
        }
        return remaining == 0;
    }

    //유닛을 조합하기 전 인벤토리 공간 체크
    private bool CheckSlot(string rcode)
    {
        
        foreach (var slot in unitSlots)
        {
            if (slot != null && slot.unitModel.rcode == rcode)
                return true;
        }
        
        foreach (var slot in unitSlots)
        {
            if (slot == null || slot.unitModel == null)
                return true;
        }
        
        return false;
    }

    // 조합 실행 함수
    public bool CombineUnit(UnionTableSO tableSO)
    {
        if (!CheckSlot(tableSO.unitRcode))
            return false;

        //뽑으려는 유닛 정보
        UnitDataSO unitData = DataManager.Instance.GetData<UnitDataSO>(tableSO.unitRcode);
        List<string> reqs = new List<string>();
        if (!string.IsNullOrEmpty(tableSO.unit1)) reqs.Add(tableSO.unit1);
        if (!string.IsNullOrEmpty(tableSO.unit2)) reqs.Add(tableSO.unit2);
        if (!string.IsNullOrEmpty(tableSO.unit3)) reqs.Add(tableSO.unit3);
        if (!string.IsNullOrEmpty(tableSO.unit4)) reqs.Add(tableSO.unit4);

        foreach (var rcode in reqs)
        {
            if (GetUnitCount(rcode) < 1)
                return false;
        }

        if (!UseSoulStone(unitData.cost))
        {
            return false;
        }

        foreach (var rcode in reqs)
        {
            ConsumeUnit(rcode, 1);
        }

        AddUnit(new UnitModel(unitData));

        return true;
    }

    public bool SellUnit(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= unitSlots.Length)
        {
            return false;
        }

        StackableUnitModel item = unitSlots[slotIndex];

        if ((int)item.unitModel.grade == 5)
        {
            return false;
        }

        SoulStone += CalculateCost(item.unitModel.cost);

        RemoveUnit(slotIndex);

        StageUIManager.Instance.RefreshUnitSlot(slotIndex);

        return true;
    }

    //판매금액 계산
    public int CalculateCost(int cost)
    {
        return Mathf.RoundToInt(cost * 2f / 3f);
    }
}
#if UNITY_EDITOR //@sm 
[CustomEditor(typeof(StageManager))]
public class StageEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (EditorApplication.isPlaying)
        {
            if (GUILayout.Button("전투 시작"))
            {
                StageManager.Instance.ChangeToDay();
            }

        }
    }
}
#endif