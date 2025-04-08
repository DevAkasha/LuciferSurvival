using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem;

public abstract class CharacterController<E> : MobileController<E> where E : CharacterEntity
{
    public virtual void OnMove(CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            Entity.moveInput = context.ReadValue<Vector2>();
        if (context.phase == InputActionPhase.Canceled)
            Entity.moveInput = Vector2.zero;
    }
}