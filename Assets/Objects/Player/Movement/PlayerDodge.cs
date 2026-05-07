using System.Collections;
using UnityEngine;

public class PlayerDodge : MonoBehaviour
{
    [SerializeField] private float dodgeDistance = 2f;
    [SerializeField] private float dodgeDuration = 0.2f;

    private Rigidbody2D rb;
    private Coroutine dodgeCoroutine;
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

    public void Dodge(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.01f)
        {
            direction = Vector2.down;
        }

        if (dodgeCoroutine != null)
        {
            StopCoroutine(dodgeCoroutine);
        }

        dodgeCoroutine = StartCoroutine(DodgeRoutine(direction.normalized));
    }

    private IEnumerator DodgeRoutine(Vector2 direction)
    {
        isDodging = true;

        float elapsedTime = 0f;
        Vector2 startPosition = rb.position;
        Vector2 targetPosition = startPosition + direction * dodgeDistance;

        while (elapsedTime < dodgeDuration)
        {
            elapsedTime += Time.fixedDeltaTime;

            float progress = elapsedTime / dodgeDuration;
            Vector2 nextPosition = Vector2.Lerp(startPosition, targetPosition, progress);

            rb.MovePosition(nextPosition);

            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(targetPosition);

        isDodging = false;
        dodgeCoroutine = null;
    }
}