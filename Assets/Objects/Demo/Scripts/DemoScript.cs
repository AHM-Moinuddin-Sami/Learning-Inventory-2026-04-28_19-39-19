using UnityEngine;

public class DemoScript : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;
    public Item[] itemsToPickup;

    public void PickupItem(int id)
    {
        bool result = inventoryManager.AddItem(itemsToPickup[id]);
        if(result == true)
        {
            Debug.Log("Item added");
        }
        else
        {
            Debug.Log("Item not added.");
        }
    }
}
