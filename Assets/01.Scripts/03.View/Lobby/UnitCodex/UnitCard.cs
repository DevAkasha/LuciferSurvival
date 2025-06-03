using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitCard : MonoBehaviour
{
    [SerializeField]
    private Image unitImage;

    [SerializeField]
    private TextMeshProUGUI gradeText;

    [SerializeField]
    private TextMeshProUGUI unitNameText;

    [SerializeField]
    private UnitCodexDetail detailPrefab;

    private UnitDataSO unitData;

    private Transform detailTransform;

    public void SetUnitCard(UnitDataSO data)
    {
        unitData = data;
        unitImage.sprite = data.thumbnail;
        gradeText.text = ((int)data.grade).ToString();  
        unitNameText.text = data.displayName;
    }

    //카드 클릭시 팝업 생성
    public void OnClcickCard()
    {
        detailPrefab.SetUnitInfo(unitData);
        detailPrefab.SetUnitSkillInfo(unitData);
        UIManager.Show(detailPrefab, detailTransform);
    }

    public void SetDetailTransform(Transform parent)
    {
        detailTransform = parent;
    }
}
