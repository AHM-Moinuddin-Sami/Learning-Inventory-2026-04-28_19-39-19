using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private InventorySlot[] inventorySlots;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private GameObject inventoryRoot;
    [SerializeField] private GameObject blackBG;
    [SerializeField] private GameObject equipmentRoot;

    private int selectedSlot = -1;
    private InventoryItem heldItem;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        SelectSlotByIndex(0);
        RefreshBlackBackground();
    }

    private void OnEnable()
    {
        if (InputManager.Instance == null)
        {
            Debug.LogWarning("InventoryManager could not find InputManager.");
            return;
        }

        InputManager.Instance.OnHotkeySlotPressed += SelectSlotByIndex;
        InputManager.Instance.OnInventoryHotkeyPressed += ToggleInventory;
        InputManager.Instance.OnEquipmentHotkeyPressed += ToggleEquipment;
    }

    private void OnDisable()
    {
        if (InputManager.Instance == null)
        {
            return;
        }

        InputManager.Instance.OnHotkeySlotPressed -= SelectSlotByIndex;
        InputManager.Instance.OnInventoryHotkeyPressed -= ToggleInventory;
        InputManager.Instance.OnEquipmentHotkeyPressed -= ToggleEquipment;
    }

    private void Update()
    {
        if (heldItem == null)
        {
            return;
        }

        heldItem.transform.position = Pointer.current.position.ReadValue();
    }

    public void PickUpItemWithCursor(InventoryItem inventoryItem)
    {
        if (inventoryItem == null || heldItem != null)
        {
            return;
        }

        HoldItem(inventoryItem);
    }

    public bool TryPlaceHeldItemInSlot(InventorySlot slot)
    {
        if (heldItem == null || slot == null)
        {
            return false;
        }

        InventoryItem itemToPlace = heldItem;
        Transform originalParent = itemToPlace.parentAfterDrag;

        if (slot.TryPlaceItem(itemToPlace, out InventoryItem[] pickedUpItems) == false)
        {
            return false;
        }

        heldItem = null;

        itemToPlace.SetRaycastTarget(true);
        itemToPlace.SnapToParentAfterDrag();

        HandlePickedUpItems(pickedUpItems, originalParent);

        InventorySlot.NotifySlotContentsChanged();

        return true;
    }

    private void HandlePickedUpItems(InventoryItem[] pickedUpItems, Transform originalParent)
    {
        if (pickedUpItems == null || pickedUpItems.Length == 0)
        {
            return;
        }

        for (int i = 0; i < pickedUpItems.Length; i++)
        {
            InventoryItem pickedUpItem = pickedUpItems[i];

            if (pickedUpItem == null)
            {
                continue;
            }

            if (heldItem == null)
            {
                HoldItem(pickedUpItem, originalParent);
                continue;
            }

            TryStoreExistingItem(pickedUpItem);
        }
    }

    public void HandlePickedUpItemsFromDrag(InventoryItem[] pickedUpItems, Transform originalParent)
    {
        HandlePickedUpItems(pickedUpItems, originalParent);
    }

    private void HoldItem(InventoryItem inventoryItem, Transform returnParent = null)
    {
        if (inventoryItem == null)
        {
            return;
        }

        heldItem = inventoryItem;

        if (returnParent == null)
        {
            heldItem.parentAfterDrag = heldItem.transform.parent;
        }
        else
        {
            heldItem.parentAfterDrag = returnParent;
        }

        heldItem.SetRaycastTarget(false);
        heldItem.transform.SetParent(heldItem.transform.root);

        InventorySlot.NotifySlotContentsChanged();
    }

    public void CancelHeldItem()
    {
        if (heldItem == null)
        {
            return;
        }

        heldItem.SetRaycastTarget(true);
        heldItem.SnapToParentAfterDrag();
        heldItem = null;

        InventorySlot.NotifySlotContentsChanged();
    }

    public void SelectSlotByIndex(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventorySlots.Length)
        {
            return;
        }

        if (selectedSlot == slotIndex)
        {
            return;
        }

        if (selectedSlot >= 0)
        {
            inventorySlots[selectedSlot].Deselect();
        }

        inventorySlots[slotIndex].Select();
        selectedSlot = slotIndex;
    }

    public bool AddItem(Item item, int amount = 1)
    {
        if (item == null || amount <= 0)
        {
            return false;
        }

        if (CanFitItem(item, amount) == false)
        {
            return false;
        }

        if (item.stackable)
        {
            AddToExistingStacks(item, ref amount);
        }

        AddToEmptySlots(item, ref amount);

        InventorySlot.NotifySlotContentsChanged();

        return true;
    }

    private bool CanFitItem(Item item, int amount)
    {
        int availableSpace = 0;

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];

            if (slot == null || slot.CanAcceptItem(item.itemType) == false)
            {
                continue;
            }

            InventoryItem itemInSlot = GetItemInSlot(slot);

            if (itemInSlot == null)
            {
                availableSpace += item.stackable ? item.maxStackCount : 1;
            }
            else if (item.stackable && itemInSlot.item == item)
            {
                availableSpace += item.maxStackCount - itemInSlot.count;
            }

            if (availableSpace >= amount)
            {
                return true;
            }
        }

        return false;
    }

    private void AddToExistingStacks(Item item, ref int amount)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventoryItem itemInSlot = GetItemInSlot(inventorySlots[i]);

            if (itemInSlot == null)
            {
                continue;
            }

            if (itemInSlot.item != item)
            {
                continue;
            }

            int availableSpace = item.maxStackCount - itemInSlot.count;

            if (availableSpace <= 0)
            {
                continue;
            }

            int amountToAdd = Mathf.Min(availableSpace, amount);

            itemInSlot.count += amountToAdd;
            itemInSlot.RefreshCount();

            amount -= amountToAdd;

            if (amount <= 0)
            {
                return;
            }
        }
    }

    private void AddToEmptySlots(Item item, ref int amount)
    {
        while (amount > 0)
        {
            InventorySlot emptySlot = GetFirstEmptyValidSlot(item);

            if (emptySlot == null)
            {
                return;
            }

            int amountToAdd = item.stackable ? Mathf.Min(item.maxStackCount, amount) : 1;

            SpawnNewItem(item, emptySlot, amountToAdd);

            amount -= amountToAdd;
        }
    }

    public bool TryStoreExistingItem(InventoryItem inventoryItem)
    {
        if (inventoryItem == null || inventoryItem.item == null)
        {
            return false;
        }

        InventorySlot slot = GetFirstEmptyValidSlot(inventoryItem.item);

        if (slot == null)
        {
            return false;
        }

        inventoryItem.transform.SetParent(slot.transform);
        inventoryItem.transform.localPosition = Vector3.zero;

        InventorySlot.NotifySlotContentsChanged();

        return true;
    }

    private InventorySlot GetFirstEmptyValidSlot(Item item)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];

            if (slot == null)
            {
                continue;
            }

            if (slot.CanAcceptItem(item.itemType) == false)
            {
                continue;
            }

            if (GetItemInSlot(slot) == null)
            {
                return slot;
            }
        }

        return null;
    }

    public InventoryItem GetItemInSlot(InventorySlot slot)
    {
        if (slot == null)
        {
            return null;
        }

        return slot.GetComponentInChildren<InventoryItem>();
    }

    private void SpawnNewItem(Item item, InventorySlot slot, int amount)
    {
        GameObject newItemGameObject = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItemGameObject.GetComponent<InventoryItem>();

        inventoryItem.InitializeItem(item);
        inventoryItem.count = amount;
        inventoryItem.RefreshCount();
    }

    public bool HasHeldItem
    {
        get
        {
            return heldItem != null;
        }
    }

    private void ToggleInventory()
    {
        inventoryRoot.SetActive(!inventoryRoot.activeSelf);
        RefreshBlackBackground();
    }

    private void ToggleEquipment()
    {
        equipmentRoot.SetActive(!equipmentRoot.activeSelf);
        RefreshBlackBackground();
    }

    private void RefreshBlackBackground()
    {
        blackBG.SetActive(inventoryRoot.activeSelf || equipmentRoot.activeSelf);
    }
}