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
            StageManager.Instance.RerollCost += 1;
        }
        else
        {
            curImage.sprite = unLockSprite;
            StageManager.Instance.RerollCost -= 1;
        }
    }
}
