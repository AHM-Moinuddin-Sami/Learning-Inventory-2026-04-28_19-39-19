using UnityEngine;

public class EquipmentStatController : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private InventorySlot[] equipmentSlots;

    private bool refreshQueued;

    private void Awake()
    {
        if (playerStats == null)
        {
            playerStats = FindAnyObjectByType<PlayerStats>();
        }
    }

    private void OnEnable()
    {
        InventorySlot.OnSlotContentsChanged += QueueRefreshEquipmentStats;
    }

    private void OnDisable()
    {
        InventorySlot.OnSlotContentsChanged -= QueueRefreshEquipmentStats;
    }

    private void Start()
    {
        RefreshEquipmentStatsImmediately();
    }

    private void LateUpdate()
    {
        if (refreshQueued == false)
        {
            return;
        }

        refreshQueued = false;
        RefreshEquipmentStatsImmediately();
    }

    public void QueueRefreshEquipmentStats()
    {
        refreshQueued = true;
    }

    public void RefreshEquipmentStatsImmediately()
    {
        if (playerStats == null)
        {
            return;
        }

        playerStats.RebuildEquipmentStats(equipmentSlots);
    }
}