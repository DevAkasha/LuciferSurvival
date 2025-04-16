using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    protected override bool IsPersistent => true;
    public PlayerController Player { get; set;}

    public void ResistPlayer(PlayerController player)
    {
        Player = player;
    }
    private void Update()
    {
        UnityTimer.Tick(Time.deltaTime);
    }
}
