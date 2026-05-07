using UnityEngine;

[CreateAssetMenu(menuName = "Player Combat/Attack Data")]
public class PlayerAttackData : ScriptableObject
{
    [Header("Animation")]
    [SerializeField] private string animationTriggerName;

    [Header("Damage")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackRadius = 0.5f;

    [Header("Timing")]
    [SerializeField] private float attackLockDuration = 0.35f;
    [SerializeField] private float comboDuration = 0.55f;
    [SerializeField] private float resetDuration = 0.8f;

    public string AnimationTriggerName
    {
        get
        {
            return animationTriggerName;
        }
    }

    public int Damage
    {
        get
        {
            return damage;
        }
    }

    public float AttackRadius
    {
        get
        {
            return attackRadius;
        }
    }

    public float AttackLockDuration
    {
        get
        {
            return attackLockDuration;
        }
    }

    public float ComboDuration
    {
        get
        {
            return comboDuration;
        }
    }

    public float ResetDuration
    {
        get
        {
            return resetDuration;
        }
    }
}