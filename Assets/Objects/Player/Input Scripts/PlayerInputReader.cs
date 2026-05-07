using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    public event Action<AttackInputType> OnAttackPressed;
    public event Action OnDodgePressed;
    public event Action OnInteractPressed;

    private Vector2 moveInput;

    public Vector2 MoveInput
    {
        get
        {
            return moveInput;
        }
    }

    public bool HasMovementInput
    {
        get
        {
            return moveInput.sqrMagnitude > 0.01f;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (moveInput.sqrMagnitude > 1f)
        {
            moveInput.Normalize();
        }
    }

    public void OnLightAttack(InputAction.CallbackContext context)
    {
        if (context.performed == false)
        {
            return;
        }

        OnAttackPressed?.Invoke(AttackInputType.Light);
    }

    public void OnHeavyAttack(InputAction.CallbackContext context)
    {
        if (context.performed == false)
        {
            return;
        }

        OnAttackPressed?.Invoke(AttackInputType.Heavy);
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.performed == false)
        {
            return;
        }

        OnDodgePressed?.Invoke();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed == false)
        {
            return;
        }

        OnInteractPressed?.Invoke();
    }
}