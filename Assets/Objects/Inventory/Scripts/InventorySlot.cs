using System;
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
    public static event Action OnSlotContentsChanged;

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

        NotifySlotContentsChanged();
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
        switch (slotRestriction)
        {
            case SlotRestriction.Any:
                return true;

            case SlotRestriction.WeaponOnly:
                return itemType == ItemType.Weapon;

            case SlotRestriction.ArmourOnly:
                return itemType == ItemType.Armour;

            case SlotRestriction.Equipment:
                return itemType == ItemType.Weapon || itemType == ItemType.Armour;

            case SlotRestriction.Consumable:
                return itemType == ItemType.Consumable;

            case SlotRestriction.Helmet:
                return itemType == ItemType.Helmet;

            case SlotRestriction.BodyArmour:
                return itemType == ItemType.BodyArmour;

            case SlotRestriction.Gloves:
                return itemType == ItemType.Gloves;

            case SlotRestriction.Leggings:
                return itemType == ItemType.Leggings;

            case SlotRestriction.Boots:
                return itemType == ItemType.Boots;

            case SlotRestriction.RightHanded:
                return itemType == ItemType.Weapon;

            case SlotRestriction.LeftHanded:
                return itemType == ItemType.Weapon;

            case SlotRestriction.Ring:
                return itemType == ItemType.Ring;

            case SlotRestriction.Amulet:
                return itemType == ItemType.Amulet;

            default:
                return false;
        }
    }

    public static void NotifySlotContentsChanged()
    {
        OnSlotContentsChanged?.Invoke();
    }
}