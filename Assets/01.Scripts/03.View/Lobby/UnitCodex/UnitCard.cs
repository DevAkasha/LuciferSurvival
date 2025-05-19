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

    //[SerializeField] 유닛 상세정보 작성 필요
    

    public void SetUnitCard(Sprite sprite, int grade, string unitName)
    {
        unitImage.sprite = sprite;
        gradeText.text = grade.ToString();
        unitNameText.text = unitName;
    }

    //카드 클릭시 팝업 생성
    public void OnCardClick()
    {

    }
}
