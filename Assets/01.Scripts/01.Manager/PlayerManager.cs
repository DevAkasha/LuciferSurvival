using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    public PlayerController Player { get; set;}

    public void ResistPlayer(PlayerController player)
    {
        Player = player;
    }
}
