using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem;

public abstract class PlayerController : MobileController<PlayerEntity, PlayerModel>
{
    [SerializeField] private Animator animator;
    public Transform unitSlots;
    [SerializeField] private Transform[] unitTransforms;
    
    protected override void AtInit()
    {
        PlayerManager.Instance.ResistPlayer(this);
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

    //유닛 생성
    public void AddUnitTransform(int index, UnitModel model)
    {
        var prefab = Resources.Load<UnitController>($"Prefabs/Unit/{model.rcode}");
        if (prefab == null)
        {
            return;
        }
        RemoveUnitTransform(index);

        Instantiate(prefab, unitTransforms[index]);
    }

    //유닛 제거
    public void RemoveUnitTransform(int index)
    {
        var unit = unitTransforms[index].GetComponentInChildren<UnitController>();
        if (unit != null)
        {
            Destroy(unit.gameObject);
        }
    }
    public void SetDirectionUnitTransform()
    {
        foreach (var unity in unitTransforms)
        {
            unity.transform.rotation = this.transform.rotation;
        }
    }
}