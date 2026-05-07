using System;
using UnityEngine;

[Serializable]
public class PlayerComboTransition
{
    [SerializeField] private AttackInputType inputType;
    [SerializeField] private PlayerAttackData nextAttack;

    public AttackInputType InputType
    {
        get
        {
            return inputType;
        }
    }

    public PlayerAttackData NextAttack
    {
        get
        {
            return nextAttack;
        }
    }
}