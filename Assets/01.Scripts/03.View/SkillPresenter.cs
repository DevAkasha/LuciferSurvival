using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class SkillPresenter : MonoBehaviour
{
    private CooldownButton[] cooldownButtonArray = new CooldownButton[5];

    private void Awake()
    {
        cooldownButtonArray = GetComponentsInChildren<CooldownButton>();
    }

    private void Start()
    {
        Initalize();
    }

    public void OnSkill1(CallbackContext context)
    {
        if (context.performed)
            cooldownButtonArray[0].HandleClick();
    }

    public void OnSkill2(CallbackContext context)
    {
        if (context.performed)
            cooldownButtonArray[1].HandleClick();
    }

    public void OnSkill3(CallbackContext context)
    {
        if (context.performed)
            cooldownButtonArray[2].HandleClick();
    }

    public void OnSkill4(CallbackContext context) 
    {
        if (context.performed)
            cooldownButtonArray[3].HandleClick();
    }

    public void OnInteract(CallbackContext context) 
    {
        if (context.performed)
            cooldownButtonArray[4].HandleClick();
    }

    public void Initalize()
    {
        var playerSkillList = PlayerManager.Instance.Player.GetComponent<PlayerSkillModule>().PlayerSkillList;
        for (int i = 0; i < 4; i++)
        {
            cooldownButtonArray[i].Initialize(playerSkillList[i]);
        }
        cooldownButtonArray[4].Initialize((1f, () => PlayerManager.Instance.Player.OnInteract()));
    }
}
