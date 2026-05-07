using System;
using UnityEngine;

[Serializable]
public class PlayerComboStep
{
    [SerializeField] private AttackInputType inputType;
    [SerializeField] private PlayerAttackData attackData;

    public AttackInputType InputType
    {
        get
        {
            return inputType;
        }
    }

    public PlayerAttackData AttackData
    {
        get
        {
            return attackData;
        }
    }
}