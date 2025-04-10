using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SummonUnitInfo : MonoBehaviour
{
    [SerializeField]
    private Image unitImage;

    [SerializeField]
    private TextMeshProUGUI gradeText;

    [SerializeField]
    private TextMeshProUGUI unitNameText;

    [SerializeField]
    private TextMeshProUGUI unitCostText;

    public void SetUnitInfo(UnitModel unit)
    {
        //유닛 이미지 선택
        //unitImage.sprite = 

        switch (unit.grade)
        {
            case UnitGrade.One :
                SetGradeText("일반");
                break;
            case UnitGrade.Two :
                SetGradeText("간부");
                break;
            case UnitGrade.Three :
                SetGradeText("고위간부");
                break;
            case UnitGrade.Four:
                SetGradeText("대장");
                break;
            default :
                break;
        }

        unitNameText.text = unit.displayName;
        unitCostText.text = unit.cost.ToString();
    }

    public void SetUnitImage(Sprite sprite)
    {
        unitImage.sprite = sprite;
    }

    public void SetGradeText(string grade)
    {
        gradeText.text = grade;
    }

    public void SetUnitNameText(string unitName)
    {
        unitNameText.text = unitName;
    }

    public void SetUnitCost(int cost)
    {
        unitCostText.text = cost.ToString();
    }
}
