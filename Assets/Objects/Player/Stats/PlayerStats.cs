using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] private float baseMaxHealth = 100f;
    [SerializeField] private float baseDamage = 0f;
    [SerializeField] private float baseDefence = 0f;
    [SerializeField] private float baseMoveSpeed = 5f;
    [SerializeField] private float baseCritChance = 0f;

    private float[] baseValues;
    private float[] flatValues;
    private float[] increasedValues;
    private float[] moreValues;
    private float[] finalValues;

    public event Action OnStatsChanged;

    private void Awake()
    {
        int statCount = Enum.GetValues(typeof(StatType)).Length;

        baseValues = new float[statCount];
        flatValues = new float[statCount];
        increasedValues = new float[statCount];
        moreValues = new float[statCount];
        finalValues = new float[statCount];

        BuildBaseValues();
        ClearModifierCaches();
        RecalculateFinalValues();
    }

    public void RebuildEquipmentStats(InventorySlot[] equipmentSlots)
    {
        ClearModifierCaches();

        if (equipmentSlots != null)
        {
            AddEquipmentModifiers(equipmentSlots);
        }

        RecalculateFinalValues();
        NotifyStatsChanged();
    }

    public float GetFinalValue(StatType statType)
    {
        int index = (int)statType;

        if (finalValues == null)
        {
            return 0f;
        }

        if (index < 0 || index >= finalValues.Length)
        {
            return 0f;
        }

        return finalValues[index];
    }

    public int GetFinalIntValue(StatType statType)
    {
        return Mathf.RoundToInt(GetFinalValue(statType));
    }

    private void BuildBaseValues()
    {
        SetBaseValue(StatType.MaxHealth, baseMaxHealth);
        SetBaseValue(StatType.Damage, baseDamage);
        SetBaseValue(StatType.Defence, baseDefence);
        SetBaseValue(StatType.MoveSpeed, baseMoveSpeed);
        SetBaseValue(StatType.CritChance, baseCritChance);
    }

    private void SetBaseValue(StatType statType, float value)
    {
        baseValues[(int)statType] = value;
    }

    private void ClearModifierCaches()
    {
        for (int i = 0; i < flatValues.Length; i++)
        {
            flatValues[i] = 0f;
            increasedValues[i] = 0f;
            moreValues[i] = 1f;
        }
    }

    private void AddEquipmentModifiers(InventorySlot[] equipmentSlots)
    {
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            InventorySlot equipmentSlot = equipmentSlots[i];

            if (equipmentSlot == null)
            {
                continue;
            }

            InventoryItem inventoryItem = equipmentSlot.GetComponentInChildren<InventoryItem>();

            if (inventoryItem == null || inventoryItem.item == null)
            {
                continue;
            }

            AddItemModifiers(inventoryItem.item);
        }
    }

    private void AddItemModifiers(Item item)
    {
        StatModifier[] statModifiers = item.StatModifiers;

        if (statModifiers == null)
        {
            return;
        }

        for (int i = 0; i < statModifiers.Length; i++)
        {
            StatModifier modifier = statModifiers[i];

            if (modifier == null)
            {
                continue;
            }

            int statIndex = (int)modifier.StatType;

            if (statIndex < 0 || statIndex >= finalValues.Length)
            {
                continue;
            }

            if (modifier.ModifierMode == StatModifierMode.Flat)
            {
                flatValues[statIndex] += modifier.Value;
            }
            else if (modifier.ModifierMode == StatModifierMode.Increased)
            {
                increasedValues[statIndex] += modifier.Value;
            }
            else if (modifier.ModifierMode == StatModifierMode.More)
            {
                moreValues[statIndex] *= 1f + (modifier.Value / 100f);
            }
        }
    }

    private void RecalculateFinalValues()
    {
        for (int i = 0; i < finalValues.Length; i++)
        {
            float value = baseValues[i] + flatValues[i];

            value *= 1f + (increasedValues[i] / 100f);
            value *= moreValues[i];

            finalValues[i] = ClampStat((StatType)i, value);
        }
    }

    private float ClampStat(StatType statType, float value)
    {
        switch (statType)
        {
            case StatType.MaxHealth:
                return Mathf.Max(1f, value);

            case StatType.MoveSpeed:
                return Mathf.Max(0f, value);

            case StatType.CritChance:
                return Mathf.Max(0f, value);

            default:
                return value;
        }
    }

    private void NotifyStatsChanged()
    {
        if (OnStatsChanged != null)
        {
            OnStatsChanged.Invoke();
        }
    }
}