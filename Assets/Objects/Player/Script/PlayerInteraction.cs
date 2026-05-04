using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float interactionDistance = 1f;
    [SerializeField] private float interactionDuration = 0.1f;
    [SerializeField] private LayerMask interactionLayer;

    public float InteractionDuration
    {
        get
        {
            return interactionDuration;
        }
    }

    public void Interact(Vector2 facingDirection)
    {
        Vector2 origin = transform.position;
        Vector2 direction = facingDirection;

        if (direction.sqrMagnitude < 0.01f)
        {
            direction = Vector2.down;
        }

        RaycastHit2D hit = Physics2D.Raycast(origin, direction.normalized, interactionDistance, interactionLayer);

        if (hit.collider == null)
        {
            return;
        }

        IInteractable interactable = hit.collider.GetComponent<IInteractable>();

        if (interactable == null)
        {
            return;
        }

        interactable.Interact();
    }
}