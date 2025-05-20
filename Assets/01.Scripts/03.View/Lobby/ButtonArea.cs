using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonArea : MonoBehaviour
{
    [SerializeField]
    private UnitCodexUI codexPrefab;

    [SerializeField]
    private Transform codexTransform;


    public void OnCodexOpen()
    {
        UIManager.Show(codexPrefab, codexTransform);
    }

}
