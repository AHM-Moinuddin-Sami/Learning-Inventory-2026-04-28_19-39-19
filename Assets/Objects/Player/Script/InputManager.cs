using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    [SerializeField] private InputActionAsset inputActions; // Input Action

    //Events
    public event Action<int> OnHotkeySlotPressed;
    public event Action OnInventoryHotkeyPressed;
    public event Action OnEquipmentHotkeyPressed;

    [Header("Action Map Names")]
    [SerializeField] private string hotkeyActionMapName = "UI";

    [Header("Action Names")]
    [SerializeField]
    private string[] hotkeyActionNames =
    {
        "Hotkey 1",
        "Hotkey 2",
        "Hotkey 3",
        "Hotkey 4",
        "Hotkey 5",
        "Hotkey 6",
        "Hotkey 7",
    };
    [SerializeField] private string inventoryHotkeyActionName = "Inventory Hotkey";
    [SerializeField] private string equipmentHotkeyActionName = "Equipment Hotkey";

    private InputActionMap hotkeyActionMap;
    private readonly Dictionary<InputAction, int> hotkeySlotsByAction = new();
    private InputAction inventoryHotkeyAction;
    private InputAction equipmentHotkeyAction;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        CacheHotkeyActions();
    }

    private void OnEnable()
    {
        RegisterHotkeyCallbacks();
        hotkeyActionMap?.Enable();
    }

    private void OnDisable()
    {
        hotkeyActionMap?.Disable();
        UnregisterHotkeyCallbacks();
    }

    private void CacheHotkeyActions()
    {
        if (inputActions == null)
        {
            Debug.LogError("InputManager is missing an InputActionAsset.");
            return;
        }

        hotkeyActionMap = inputActions.FindActionMap(hotkeyActionMapName, false);

        if (hotkeyActionMap == null)
        {
            Debug.LogError("InputManager couldnt find action map: " + hotkeyActionMapName);
            return;
        }

        // Toolbar section start
        hotkeySlotsByAction.Clear();

        for (int i = 0; i < hotkeyActionNames.Length; i++)
        {
            InputAction hotkeyAction = hotkeyActionMap.FindAction(hotkeyActionNames[i], false);
            if (hotkeyAction == null)
            {
                Debug.LogWarning("InputManager couldnt find hotkey action: " + hotkeyActionNames[i]);
                continue;
            }

            hotkeySlotsByAction.Add(hotkeyAction, i);
        }

        // toolbar section end

        inventoryHotkeyAction = hotkeyActionMap.FindAction(inventoryHotkeyActionName);
        equipmentHotkeyAction = hotkeyActionMap.FindAction(equipmentHotkeyActionName);
    }

    private void RegisterHotkeyCallbacks()
    {
        foreach (KeyValuePair<InputAction, int> hotkeyAction in hotkeySlotsByAction) // Toolbar Section
        {
            hotkeyAction.Key.performed += HandleHotkeyPressed;
        }

        if (inventoryHotkeyAction != null)
        {
            inventoryHotkeyAction.performed += HandleInventoryKeyPressed;
        }

        if (equipmentHotkeyAction != null)
        {
            equipmentHotkeyAction.performed += HandleEquipmentKeyPressed;
        }
    }

    private void UnregisterHotkeyCallbacks()
    {
        foreach (KeyValuePair<InputAction, int> hotkeyAction in hotkeySlotsByAction) // Toolbar Section
        {
            hotkeyAction.Key.performed -= HandleHotkeyPressed;
        }

        if (inventoryHotkeyAction != null)
        {
            inventoryHotkeyAction.performed -= HandleInventoryKeyPressed;
        }

        if (equipmentHotkeyAction != null)
        {
            equipmentHotkeyAction.performed -= HandleEquipmentKeyPressed;
        }
    }

    private void HandleHotkeyPressed(InputAction.CallbackContext context)
    {
        Debug.Log("Hotkey action triggered: " + context.action.name);

        if (hotkeySlotsByAction.TryGetValue(context.action, out int slotIndex) == false)
        {
            return;
        }

        Debug.Log("Hotkey slot index: " + slotIndex);

        OnHotkeySlotPressed?.Invoke(slotIndex);
    }

    private void HandleInventoryKeyPressed(InputAction.CallbackContext context)
    {
        Debug.Log("Inventory hotkey action triggered: " + context.action.name);
        OnInventoryHotkeyPressed?.Invoke();
    }

    private void HandleEquipmentKeyPressed(InputAction.CallbackContext context)
    {
        Debug.Log("Equipment hotkey action triggered: " + context.action.name);
        OnEquipmentHotkeyPressed?.Invoke();
    }
}
