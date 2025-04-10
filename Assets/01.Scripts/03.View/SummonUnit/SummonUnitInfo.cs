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
        gradeText.text = unit.displayName;
        unitNameText.text = unit.displayName;

    }

    public void SetUnitImage(Sprite sprite)
    {
        unitImage.sprite = sprite;
    }

    public void SetGradeText(string grade)
    {
        gradeText.text = grade;
    }

}
