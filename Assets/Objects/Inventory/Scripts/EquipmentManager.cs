using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [SerializeField] private InventorySlot rightHandSlot;
    [SerializeField] private InventorySlot leftHandSlot;
    [SerializeField] private InventoryManager inventoryManager;

    public bool TryEquipWeapon(InventoryItem draggedItem, InventorySlot droppedSlot, out InventoryItem pickedUpItem)
    {
        pickedUpItem = null;

        if (draggedItem == null || draggedItem.item == null)
        {
            return false;
        }

        if (draggedItem.item.itemType != ItemType.Weapon)
        {
            return false;
        }

        if (draggedItem.item.weaponHandType == WeaponHandType.TwoHanded)
        {
            return EquipTwoHandedWeapon(draggedItem, out pickedUpItem);
        }

        if (draggedItem.item.weaponHandType == WeaponHandType.OneHanded)
        {
            return EquipOneHandedWeapon(draggedItem, droppedSlot, out pickedUpItem);
        }

        return false;
    }

    private bool EquipTwoHandedWeapon(InventoryItem draggedItem, out InventoryItem pickedUpItem)
    {
        pickedUpItem = null;

        if (rightHandSlot == null || leftHandSlot == null)
        {
            return false;
        }

        InventoryItem rightHandItem = GetItemInSlot(rightHandSlot);
        InventoryItem leftHandItem = GetItemInSlot(leftHandSlot);

        if (leftHandItem != null && leftHandItem != draggedItem)
        {
            if (inventoryManager.TryStoreExistingItem(leftHandItem) == false)
            {
                return false;
            }
        }

        if (rightHandItem != null && rightHandItem != draggedItem)
        {
            pickedUpItem = rightHandItem;
        }

        draggedItem.parentAfterDrag = rightHandSlot.transform;
        return true;
    }

    private bool EquipOneHandedWeapon(InventoryItem draggedItem, InventorySlot droppedSlot, out InventoryItem pickedUpItem)
    {
        pickedUpItem = null;

        if (rightHandSlot == null || leftHandSlot == null)
        {
            return false;
        }

        InventoryItem rightHandItem = GetItemInSlot(rightHandSlot);

        if (IsTwoHandedWeapon(rightHandItem) && rightHandItem != draggedItem)
        {
            pickedUpItem = rightHandItem;
            draggedItem.parentAfterDrag = rightHandSlot.transform;
            return true;
        }

        InventorySlot targetSlot = GetOneHandedTargetSlot(droppedSlot, draggedItem);

        if (targetSlot == null)
        {
            return false;
        }

        InventoryItem targetItem = GetItemInSlot(targetSlot);

        if (targetItem != null && targetItem != draggedItem)
        {
            pickedUpItem = targetItem;
        }

        draggedItem.parentAfterDrag = targetSlot.transform;
        return true;
    }

    private InventorySlot GetOneHandedTargetSlot(InventorySlot droppedSlot, InventoryItem draggedItem) // returns whether or not the one handed weapon should go on the left hand slot or the right hand slot
    {
        // gets the items on both slots
        InventoryItem rightHandItem = GetItemInSlot(rightHandSlot);
        InventoryItem leftHandItem = GetItemInSlot(leftHandSlot);

        if (rightHandItem == null || rightHandItem == draggedItem) // checks if right hand slot is empty, returns right hand slot if empty
        {
            return rightHandSlot;
        }

        if (droppedSlot == leftHandSlot && (leftHandItem == null || leftHandItem == draggedItem)) // if the dropped slot if left hand slot and its empty, return left hand slot
        {
            return leftHandSlot;
        }

        if (leftHandItem == null || leftHandItem == draggedItem) // catch all return left hand slot if empty and no errors
        {
            return leftHandSlot;
        }

        return droppedSlot; // catch all return right hand slot, mainly if right hand is empty and dropped slot if left hand slot
    }

    private bool MoveSlotItemToInventory(InventorySlot slot, InventoryItem ignoredItem)
    {
        InventoryItem itemInSlot = GetItemInSlot(slot);

        if (itemInSlot == null || itemInSlot == ignoredItem)
        {
            return true;
        }

        return inventoryManager.TryStoreExistingItem(itemInSlot);
    }

    private InventoryItem GetItemInSlot(InventorySlot slot)
    {
        return slot != null ? slot.GetComponentInChildren<InventoryItem>() : null;
    }

    private bool IsTwoHandedWeapon(InventoryItem inventoryItem)
    {
        if (inventoryItem == null || inventoryItem.item == null)
        {
            return false;
        }

        return inventoryItem.item.itemType == ItemType.Weapon &&
               inventoryItem.item.weaponHandType == WeaponHandType.TwoHanded;
    }
}