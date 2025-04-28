using System.Collections;
using System.Collections.Generic;
using Ricimi;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Lock : MonoBehaviour
{
    [SerializeField]
    public Image curImage;

    [SerializeField]
    private Sprite unLockSprite;

    [SerializeField]
    private Sprite lockedSprite;

    public bool isLocked = false;

    public void OnToggleCardLock()
    {
        isLocked = !isLocked;
        if (isLocked)
        {
            curImage.sprite = lockedSprite;
            //금액 올라가는 함수 구현 필요
        }
        else
        {
            curImage.sprite = unLockSprite;
            //금액 내려가는 함수 구현 필요
        }
    }
}
