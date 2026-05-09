using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerAnimator playerAnimator;

    [Header("Combos")]
    [SerializeField] private PlayerComboData[] combos;

    [Header("Hitbox")]
    [SerializeField] private float attackPointDistance = 0.7f;

    [Header("Input Buffer")]
    [SerializeField] private int maxComboInputLength = 6;

    private Animator animator;
    private PlayerMovement playerMovement;

    private PlayerAttackData currentAttack;
    private PlayerComboData currentCombo;

    private AttackInputType[] inputSequence;
    private int inputSequenceLength;
    private int currentComboStepIndex;

    private float comboTimer;
    private float resetTimer;

    public PlayerAttackData CurrentAttack
    {
        get
        {
            return currentAttack;
        }
    }

    public float CurrentAttackLockDuration
    {
        get
        {
            if (currentAttack == null)
            {
                return 0f;
            }

            return currentAttack.AttackLockDuration;
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();

        if (playerStats == null)
        {
            playerStats = GetComponent<PlayerStats>();
        }

        if (playerAnimator == null)
        {
            playerAnimator = GetComponent<PlayerAnimator>();
        }

        inputSequence = new AttackInputType[maxComboInputLength];
    }

    private void Update()
    {
        UpdateAttackPointPosition();
        UpdateComboTimers();
    }

    public bool TryStartAttack(AttackInputType inputType)
    {
        if (comboTimer <= 0f)
        {
            ResetCombo();
        }

        AddInputToSequence(inputType);

        PlayerComboData matchingCombo = FindBestMatchingCombo();

        if (matchingCombo == null)
        {
            ResetCombo();

            AddInputToSequence(inputType);
            matchingCombo = FindBestMatchingCombo();

            if (matchingCombo == null)
            {
                Debug.LogWarning("No combo starts with input: " + inputType);
                return false;
            }
        }

        int attackIndex = inputSequenceLength - 1;
        PlayerAttackData attackData = matchingCombo.GetAttackAtStep(attackIndex);

        if (attackData == null)
        {
            Debug.LogWarning("Matching combo has no attack data at step: " + attackIndex);
            return false;
        }

        currentCombo = matchingCombo;
        currentComboStepIndex = attackIndex;

        PlayAttack(attackData);
        return true;
    }

    public void ApplyAttackHit()
    {
        if (currentAttack == null)
        {
            return;
        }

        if (attackPoint == null)
        {
            Debug.LogWarning("PlayerCombat is missing attackPoint.");
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            currentAttack.AttackRadius,
            enemyLayer
        );

        int finalDamage = GetFinalAttackDamage(currentAttack);

        for (int i = 0; i < hits.Length; i++)
        {
            Debug.Log("Hit enemy: " + hits[i].gameObject.name + " for " + finalDamage + " damage.");
        }
    }

    public void ClearCurrentAttack()
    {
        currentAttack = null;
    }

    public void ResetCombo()
    {
        currentAttack = null;
        currentCombo = null;
        inputSequenceLength = 0;
        currentComboStepIndex = 0;
        comboTimer = 0f;
        resetTimer = 0f;
    }

    private void PlayAttack(PlayerAttackData attackData)
    {
        currentAttack = attackData;

        comboTimer = attackData.ComboDuration;
        resetTimer = attackData.ResetDuration;

        if (playerAnimator != null)
        {
            playerAnimator.PlayAttackAnimation(attackData);
        }
        else
        {
            animator.ResetTrigger(attackData.AnimationTriggerName);
            animator.SetTrigger(attackData.AnimationTriggerName);
        }

        Debug.Log("Playing attack: " + attackData.name + " with animation step " + attackData.AnimationStep);
    }

    private int GetFinalAttackDamage(PlayerAttackData attackData)
    {
        if (attackData == null)
        {
            return 0;
        }

        float bonusDamage = 0f;

        if (playerStats != null)
        {
            bonusDamage = playerStats.GetFinalValue(StatType.Damage);
        }

        return Mathf.RoundToInt(attackData.Damage + bonusDamage);
    }

    private void AddInputToSequence(AttackInputType inputType)
    {
        if (inputSequenceLength >= inputSequence.Length)
        {
            ShiftInputSequenceLeft();
            inputSequence[inputSequence.Length - 1] = inputType;
            return;
        }

        inputSequence[inputSequenceLength] = inputType;
        inputSequenceLength++;
    }

    private void ShiftInputSequenceLeft()
    {
        for (int i = 1; i < inputSequence.Length; i++)
        {
            inputSequence[i - 1] = inputSequence[i];
        }
    }

    private PlayerComboData FindBestMatchingCombo()
    {
        PlayerComboData bestCombo = null;
        int bestStepCount = -1;

        for (int i = 0; i < combos.Length; i++)
        {
            PlayerComboData combo = combos[i];

            if (combo == null || combo.IsEmpty == true)
            {
                continue;
            }

            if (combo.MatchesInputSequence(inputSequence, inputSequenceLength) == false)
            {
                continue;
            }

            if (combo.StepCount > bestStepCount)
            {
                bestCombo = combo;
                bestStepCount = combo.StepCount;
            }
        }

        return bestCombo;
    }

    private void UpdateComboTimers()
    {
        if (comboTimer > 0f)
        {
            comboTimer -= Time.deltaTime;
        }

        if (resetTimer > 0f)
        {
            resetTimer -= Time.deltaTime;

            if (resetTimer <= 0f)
            {
                ResetCombo();
            }
        }
    }

    private void UpdateAttackPointPosition()
    {
        if (attackPoint == null)
        {
            return;
        }

        Vector2 facingDirection = playerMovement.FacingDirection;

        if (facingDirection.sqrMagnitude < 0.01f)
        {
            facingDirection = Vector2.down;
        }

        attackPoint.localPosition = facingDirection.normalized * attackPointDistance;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }

        float radius = 0.5f;

        if (currentAttack != null)
        {
            radius = currentAttack.AttackRadius;
        }

        Gizmos.DrawWireSphere(attackPoint.position, radius);
    }
}