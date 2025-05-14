using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialCard : MonoBehaviour
{
    [SerializeField]
    private Image checkImage;

    private bool isOwned;

    [SerializeField]
    private Image unitImage;

    //유닛 이미지 세팅
    public void SetUnitImage(Sprite sprite)
    {
        if (sprite != null)
        {
            unitImage.sprite = sprite;
        }
    }

    //유닛 소유 여부
    public void SetIsOwned(bool own)
    {
        isOwned = own;
        checkImage.gameObject.SetActive(own);
    }
}
