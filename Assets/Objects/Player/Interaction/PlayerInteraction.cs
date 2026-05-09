using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float interactionDistance = 1f;
    [SerializeField] private float interactionRadius = 0.25f;
    [SerializeField] private LayerMask interactionLayer;

    public bool TryGetInteractable(Vector2 facingDirection, out IInteractable interactable)
    {
        interactable = null;

        Vector2 origin = transform.position;
        Vector2 direction = GetCardinalDirection(facingDirection);

        Debug.DrawRay(origin, direction * interactionDistance, Color.red, 0.5f);

        RaycastHit2D hit = Physics2D.CircleCast(
            origin,
            interactionRadius,
            direction,
            interactionDistance,
            interactionLayer
        );

        if (hit.collider == null)
        {
            Debug.Log("Interaction circle cast hit nothing.");
            return false;
        }

        Debug.Log("Interaction circle cast hit: " + hit.collider.gameObject.name);

        interactable = hit.collider.GetComponent<IInteractable>();

        if (interactable == null)
        {
            Debug.Log("Hit object does not have an IInteractable script.");
            return false;
        }

        return true;
    }

    private Vector2 GetCardinalDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.01f)
        {
            return Vector2.down;
        }

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector2 origin = transform.position;
        Vector2 direction = Vector2.down;

        Gizmos.DrawWireSphere(origin + direction * interactionDistance, interactionRadius);
    }
}