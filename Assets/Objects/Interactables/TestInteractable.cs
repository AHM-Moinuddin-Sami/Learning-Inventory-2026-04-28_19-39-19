using UnityEngine;

public class TestInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string itemName = "Potion";
    [SerializeField] private InteractionType interactionType = InteractionType.NonBlocking;
    [SerializeField] private Item item;
    public InteractionType InteractionType
    {
        get
        {
            return interactionType;
        }
    }

    public float InteractionDuration
    {
        get
        {
            return 0f;
        }
    }

    public void Interact()
    {
        InventoryManager.Instance.AddItem(item);

        Destroy(gameObject);
    }
}