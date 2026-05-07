using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float interactionDistance = 1f;
    [SerializeField] private LayerMask interactionLayer;

    public bool TryGetInteractable(Vector2 facingDirection, out IInteractable interactable)
    {
        interactable = null;

        Vector2 origin = transform.position;
        Vector2 direction = facingDirection;

        if (direction.sqrMagnitude < 0.01f)
        {
            direction = Vector2.down;
        }

        Debug.DrawRay(origin, direction.normalized * interactionDistance, Color.red, 0.5f);

        RaycastHit2D hit = Physics2D.Raycast(origin, direction.normalized, interactionDistance, interactionLayer);

        if (hit.collider == null)
        {
            Debug.Log("Interaction raycast hit nothing.");
            return false;
        }

        Debug.Log("Interaction raycast hit: " + hit.collider.gameObject.name);

        interactable = hit.collider.GetComponent<IInteractable>();

        if (interactable == null)
        {
            Debug.Log("Hit object does not have an IInteractable script.");
            return false;
        }

        return true;
    }
}