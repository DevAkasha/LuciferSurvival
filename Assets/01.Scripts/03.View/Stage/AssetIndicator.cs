using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AssetIndicator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI soulStoneTxt;
    [SerializeField] private TextMeshProUGUI soulCoreTxt;

    private void Start()
    {
        StageManager.Instance.soulStone.AddListener(RefreshSoulStone);
        StageManager.Instance.soulCore.AddListener(RefreshSoulCore);
    }

    private void RefreshSoulStone(int value)
    {
        soulStoneTxt.text = value.ToString();
    }

    private void RefreshSoulCore(int value)
    {
        soulCoreTxt.text = value.ToString();
    }
}
