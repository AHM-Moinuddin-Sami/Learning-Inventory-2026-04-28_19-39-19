using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum SlotRestriction
{
    Any,
    WeaponOnly,
    ArmourOnly,
    Equipment,
    Consumable,

    //Equipment slot restrictions
    Helmet,
    BodyArmour,
    Gloves,
    Leggings,
    Boots,
    Ring,
    Amulet,
    RightHanded,
    LeftHanded
}

public class InventorySlot : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    [SerializeField] private Image image;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color notSelectedColor;
    [SerializeField] private SlotRestriction slotRestriction = SlotRestriction.Any;
    [SerializeField] private EquipmentManager equipmentManager;

    public SlotRestriction SlotType
    {
        get
        {
            return slotRestriction;
        }
    }


    public void Awake()
    {
        Deselect();
    }
    public void Select()
    {
        image.color = selectedColor;
    }

    public void Deselect()
    {
        image.color = notSelectedColor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
        {
            return;
        }

        InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();

        if (inventoryItem == null)
        {
            return;
        }

        Transform originalParent = inventoryItem.parentAfterDrag;

        if (TryPlaceItem(inventoryItem, out InventoryItem[] pickedUpItems) == false)
        {
            return;
        }

        inventoryItem.SnapToParentAfterDrag();

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.HandlePickedUpItemsFromDrag(pickedUpItems, originalParent);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (InventoryManager.Instance == null)
        {
            return;
        }

        InventoryManager.Instance.TryPlaceHeldItemInSlot(this);
    }

    public bool TryPlaceItem(InventoryItem inventoryItem, out InventoryItem[] pickedUpItems)
    {
        pickedUpItems = null;

        if (inventoryItem == null || inventoryItem.item == null)
        {
            return false;
        }

        if (equipmentManager != null &&
            (slotRestriction == SlotRestriction.RightHanded || slotRestriction == SlotRestriction.LeftHanded))
        {
            return equipmentManager.TryEquipWeapon(inventoryItem, this, out pickedUpItems);
        }

        if (CanAcceptItem(inventoryItem.item.itemType) == false)
        {
            return false;
        }

        InventoryItem currentItem = GetComponentInChildren<InventoryItem>();

        if (currentItem != null && currentItem != inventoryItem)
        {
            pickedUpItems = new InventoryItem[] { currentItem };
        }

        inventoryItem.parentAfterDrag = transform;
        return true;
    }

    public bool CanAcceptItem(ItemType itemType)
    {
        return slotRestriction switch
        {
            SlotRestriction.Any => true,

            SlotRestriction.WeaponOnly => itemType == ItemType.Weapon,
            SlotRestriction.ArmourOnly => itemType == ItemType.Armour,
            SlotRestriction.Equipment => itemType == ItemType.Weapon || itemType == ItemType.Armour,

            SlotRestriction.Consumable => itemType == ItemType.Consumable,

            // Specific Equipment
            SlotRestriction.Helmet => itemType == ItemType.Helmet,
            SlotRestriction.BodyArmour => itemType == ItemType.BodyArmour,
            SlotRestriction.Gloves => itemType == ItemType.Gloves,
            SlotRestriction.Leggings => itemType == ItemType.Leggings,
            SlotRestriction.Boots => itemType == ItemType.Boots,

            SlotRestriction.RightHanded => itemType == ItemType.Weapon,
            SlotRestriction.LeftHanded => itemType == ItemType.Weapon,

            SlotRestriction.Ring => itemType == ItemType.Ring,
            SlotRestriction.Amulet => itemType == ItemType.Amulet,

            _ => false
        };
    }
}