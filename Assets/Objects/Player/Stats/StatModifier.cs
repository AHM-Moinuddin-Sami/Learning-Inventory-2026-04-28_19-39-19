using UnityEngine;

public enum StatModifierMode
{
    Flat,
    Increased,
    More
}

[System.Serializable]
public class StatModifier
{
    [SerializeField] private StatType statType;
    [SerializeField] private StatModifierMode modifierMode;
    [SerializeField] private float value;

    public StatType StatType
    {
        get
        {
            return statType;
        }
    }

    public StatModifierMode ModifierMode
    {
        get
        {
            return modifierMode;
        }
    }

    public float Value
    {
        get
        {
            return value;
        }
    }
}