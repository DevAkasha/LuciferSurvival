using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPresenter : MonoBehaviour
{
    private CooldownButton[] cooldownButtonArray = new CooldownButton[4];

    private void Awake()
    {
        cooldownButtonArray = GetComponentsInChildren<CooldownButton>();
    }

    private void Start()
    {
        Initalize();
    }

    public void Initalize()
    {
        var playerSkillList = PlayerManager.Instance.Player.GetComponent<PlayerSkillModule>().PlayerSkillList;
        for (int i = 0; i < cooldownButtonArray.Length; i++)
        {
            cooldownButtonArray[i].Initialize(playerSkillList[i]);
        }
    }
}
