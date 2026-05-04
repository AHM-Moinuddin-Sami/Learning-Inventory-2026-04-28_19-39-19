using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    public event Action OnAttackPressed;
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

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed == false)
        {
            return;
        }

        OnAttackPressed?.Invoke();
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.performed == false)
        {
            return;
        }

        Debug.Log("Dodge input pressed.");

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