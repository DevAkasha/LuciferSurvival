using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    protected override bool IsPersistent => true;
    public PlayerController Player { get; set;}
    public PlayerHealthBarView healthBar { get; set; }

    public void ResistPlayer(PlayerController player)
    {
        Player = player;
        if(healthBar != null) healthBar.Init(Player);
    }

    public void ResistPlayerHealthBar(PlayerHealthBarView healthBar)
    {
        this.healthBar = healthBar;
        if (Player != null) healthBar.Init(Player);
    }

    private void Update()
    {
        UnityTimer.Tick(Time.deltaTime);
    }
}
