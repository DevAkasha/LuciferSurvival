using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem;

public abstract class PlayerController : MobileController<PlayerEntity, PlayerModel>
{
    [SerializeField] private Animator animator;
<<<<<<< Updated upstream
=======
    public Transform unitSlots;
    [SerializeField] private Transform[] unitTransforms;
>>>>>>> Stashed changes
    
    protected override void AtInit()
    {
        PlayerManager.Instance.ResistPlayer(this);
    }
    private void Start()
    {
        Entity.Model.State.OnEnter(PlayerState.Move, () => animator.Play("Move"));
        Entity.Model.State.OnEnter(PlayerState.Idle, () => animator.Play("Idle"));
        Entity.Model.State.OnEnter(PlayerState.Death, () => animator.Play("Death"));
        Entity.Model.State.OnEnter(PlayerState.Stun, () => animator.Play("Stun"));
        Entity.Model.State.OnEnter(PlayerState.Roll, () => animator.Play("Roll"));
        Entity.Model.State.OnEnter(PlayerState.Attack, () => animator.Play("Attack"));
        Entity.Model.State.OnEnter(PlayerState.Cast, () => animator.Play("Cast"));
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