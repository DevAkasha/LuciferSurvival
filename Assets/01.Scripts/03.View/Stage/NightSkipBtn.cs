using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class NightSkipBtn : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Button button;

    public void ChangeDay()
    {
        StageManager.Instance.ChangeToDay();
    }
}