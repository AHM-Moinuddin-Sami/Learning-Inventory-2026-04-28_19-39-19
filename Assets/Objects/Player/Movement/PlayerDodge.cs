using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDodge : MonoBehaviour
{
    [Header("Dodge")]
    [SerializeField] private float dodgeDistance = 2f;
    [SerializeField] private float dodgeDuration = 0.25f;
    [SerializeField] private AnimationCurve dodgeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Rigidbody2D rb;

    private Vector2 startPosition;
    private Vector2 targetPosition;
    private Vector2 dodgeDirection;

    private float dodgeTimer;
    private bool isDodging;

    public bool IsDodging
    {
        get
        {
            return isDodging;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void BeginDodge(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.01f)
        {
            direction = Vector2.down;
        }

        dodgeDirection = GetCardinalDirection(direction);
        startPosition = rb.position;
        targetPosition = startPosition + dodgeDirection * dodgeDistance;

        dodgeTimer = 0f;
        isDodging = true;
    }

    public void FixedTickDodge()
    {
        if (isDodging == false)
        {
            return;
        }

        dodgeTimer += Time.fixedDeltaTime;

        float normalizedTime = 1f;

        if (dodgeDuration > 0f)
        {
            normalizedTime = Mathf.Clamp01(dodgeTimer / dodgeDuration);
        }

        float curveValue = dodgeCurve.Evaluate(normalizedTime);
        Vector2 newPosition = Vector2.Lerp(startPosition, targetPosition, curveValue);

        rb.MovePosition(newPosition);

        if (normalizedTime >= 1f)
        {
            isDodging = false;
        }
    }

    public void StopDodge()
    {
        isDodging = false;
    }

    private Vector2 GetCardinalDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0f)
            {
                return Vector2.right;
            }

            return Vector2.left;
        }

        if (direction.y > 0f)
        {
            return Vector2.up;
        }

        return Vector2.down;
    }
}