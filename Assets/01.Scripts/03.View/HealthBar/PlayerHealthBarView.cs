using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarView : MonoBehaviour
{
    [SerializeField] private Image fill;
    private Action<float> hpListener;
    [SerializeField] private PlayerController player;

    private void Start()
    {
        PlayerManager.Instance.ResistPlayerHealthBar(this);
    }
    public void Init(PlayerController player)
    {
        this.player = player;
        hpListener = v => fill.fillAmount = v;
        player.Entity.Model.NormalizedHP.AddListener(hpListener);
    }

    private void OnDestroy()
    {
        player.Entity.Model.NormalizedHP.RemoveListener(hpListener);
        player = null;
        PlayerManager.Instance.healthBar = null;
    }
}
