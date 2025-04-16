using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem;

public abstract class PlayerController : MobileController<PlayerEntity, PlayerModel>
{
    protected override void OnInit()
    {
        PlayerManager.Instance.ResistPlayer(this);
    }

    public virtual void OnMove(CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            Entity.moveInput = context.ReadValue<Vector2>();           
        if (context.phase == InputActionPhase.Canceled)
            Entity.moveInput = Vector2.zero;
    }

    public virtual void OnMoveByJoystick(Vector2 JoysticInput)
    {
        Entity.moveInput = JoysticInput;
    }
}