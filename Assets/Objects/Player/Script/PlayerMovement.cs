using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 facingDirection = Vector2.down;

    public Vector2 MoveInput
    {
        get
        {
            return moveInput;
        }
    }

    public Vector2 FacingDirection
    {
        get
        {
            return facingDirection;
        }
    }

    public bool IsMoving
    {
        get
        {
            return moveInput.sqrMagnitude > 0.01f;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetMoveInput(Vector2 newMoveInput)
    {
        moveInput = newMoveInput;

        if (moveInput.sqrMagnitude > 1f)
        {
            moveInput.Normalize();
        }

        if (moveInput.sqrMagnitude > 0.01f)
        {
            facingDirection = moveInput.normalized;
        }
    }

    public void StopMovement()
    {
        moveInput = Vector2.zero;
    }

    public void TickMovement()
    {
        rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * moveInput);
    }
}