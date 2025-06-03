using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitCodexDetail : UIBase
{
    [SerializeField]
    private Image unitImage;

    [SerializeField]
    private TextMeshProUGUI unitInfoText;

    [SerializeField]
    private Image skillImage;

    [SerializeField]
    private TextMeshProUGUI skillInfoText;

    public void SetUnitInfo(UnitDataSO data)
    {
        unitImage.sprite = data.thumbnail;
        unitInfoText.text = $"{data.displayName}\n공격력: {data.atk}\n공격 거리 : {(data.atkRange ? "근거리" : "원거리")}\n공격 쿨타임 : {data.atkCoolTime}";
    }

    public void SetUnitSkillInfo(UnitDataSO data)  //스킬관련 정보 받는거는 잠시 보류
    {
        //skillImage.sprite =                       //이미지 작업 필요
        //skillTitleText.text =                     //스킬 제목
        //skillInfoText.text =                      //스킬설명
    }

    public override void HideDirect()
    {
        
    }

    public override void Opened(object[] param)
    {
        
    }

    public void OnClcickClose()
    {
        UIManager.Hide(this);
    }
}
