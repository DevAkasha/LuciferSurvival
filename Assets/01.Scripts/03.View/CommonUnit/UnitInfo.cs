using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfo : MonoBehaviour
{
    [SerializeField]
    private Image unitImage;
    [SerializeField]
    private TextMeshProUGUI unitInfoText;
    [SerializeField]
    private Image skillImage;
    [SerializeField]
    private TextMeshProUGUI skillTitleText;
    [SerializeField]
    private TextMeshProUGUI skillInfoText;

    public void SetUnitInfo(UnitModel model)
    {
        //unitImage.sprite =                        //이미지 작업 필요
        unitInfoText.text = $"{model.displayName}\n공격력: {model.atk}\n사거리: {model.range}\n{model.atkSpeed}/s";
    }

    public void SetUnitSkillInfo()  //스킬관련 정보 받는거는 잠시 보류
    {
        //skillImage.sprite =                       //이미지 작업 필요
        //skillTitleText.text =                     //스킬 제목
        //skillInfoText.text =                      //스킬설명
    }
}
