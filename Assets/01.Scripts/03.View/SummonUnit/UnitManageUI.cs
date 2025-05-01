using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManageUI : MonoBehaviour
{
    [SerializeField]
    private SummonUnitUI summonUnitUI;
    [SerializeField]
    private UnitInfo unitInfo;
    [SerializeField]
    private Transform bottom;

    public SummonUnitUI SummonUnitUI { get { return summonUnitUI; } }

    public UnitInfo UnitInfo { get { return unitInfo; } }

    public Transform Bottom { get { return bottom; } }
}
