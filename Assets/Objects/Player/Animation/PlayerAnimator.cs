using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerStateMachine))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerAnimator : MonoBehaviour
{
    private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    private static readonly int MoveYHash = Animator.StringToHash("MoveY");
    private static readonly int FacingXHash = Animator.StringToHash("FacingX");
    private static readonly int FacingYHash = Animator.StringToHash("FacingY");

    private static readonly int AttackXHash = Animator.StringToHash("AttackX");
    private static readonly int AttackYHash = Animator.StringToHash("AttackY");
    private static readonly int AttackStepHash = Animator.StringToHash("AttackStep");

    private static readonly int DodgeXHash = Animator.StringToHash("DodgeX");
    private static readonly int DodgeYHash = Animator.StringToHash("DodgeY");
    private static readonly int DodgeHash = Animator.StringToHash("Dodge");

    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
    private static readonly int IsDodgingHash = Animator.StringToHash("IsDodging");
    private static readonly int IsInteractingHash = Animator.StringToHash("IsInteracting");
    private static readonly int IsStunnedHash = Animator.StringToHash("IsStunned");
    private static readonly int IsDeadHash = Animator.StringToHash("IsDead");

    private PlayerMovement playerMovement;
    private PlayerStateMachine stateMachine;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector2 lastFacingDirection = Vector2.down;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        stateMachine = GetComponent<PlayerStateMachine>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        UpdateFacingDirection();
        UpdateMovementAnimation();
        UpdateStateAnimation();
        HandleSpriteFlip();
    }

    public void PlayAttackAnimation(PlayerAttackData attackData)
    {
        if (attackData == null)
        {
            return;
        }

        animator.SetFloat(AttackXHash, lastFacingDirection.x);
        animator.SetFloat(AttackYHash, lastFacingDirection.y);
        animator.SetFloat(AttackStepHash, attackData.AnimationStep);

        if (string.IsNullOrEmpty(attackData.AnimationTriggerName))
        {
            Debug.LogWarning("Attack data is missing an animation trigger name: " + attackData.name);
            return;
        }

        animator.ResetTrigger(attackData.AnimationTriggerName);
        animator.SetTrigger(attackData.AnimationTriggerName);
    }

    public void PlayDodgeAnimation(Vector2 dodgeDirection)
    {
        Vector2 cardinalDirection = GetCardinalDirection(dodgeDirection);

        animator.SetFloat(DodgeXHash, cardinalDirection.x);
        animator.SetFloat(DodgeYHash, cardinalDirection.y);

        animator.ResetTrigger(DodgeHash);
        animator.SetTrigger(DodgeHash);
    }

    public void FinishAttack()
    {
        stateMachine.FinishAttack();
    }

    public void FinishDodge()
    {
        stateMachine.FinishDodge();
    }

    public void FinishInteraction()
    {
        stateMachine.FinishInteraction();
    }

    private void UpdateFacingDirection()
    {
        Vector2 moveInput = playerMovement.MoveInput;

        if (moveInput.sqrMagnitude <= 0.01f)
        {
            return;
        }

        lastFacingDirection = GetCardinalDirection(moveInput);
    }

    private void UpdateMovementAnimation()
    {
        Vector2 moveInput = playerMovement.MoveInput;

        animator.SetFloat(MoveXHash, moveInput.x);
        animator.SetFloat(MoveYHash, moveInput.y);

        animator.SetFloat(FacingXHash, lastFacingDirection.x);
        animator.SetFloat(FacingYHash, lastFacingDirection.y);

        animator.SetBool(IsMovingHash, playerMovement.IsMoving);
    }

    private void UpdateStateAnimation()
    {
        animator.SetBool(IsAttackingHash, stateMachine.CurrentStateType == PlayerState.Attacking);
        animator.SetBool(IsDodgingHash, stateMachine.CurrentStateType == PlayerState.Dodging);
        animator.SetBool(IsInteractingHash, stateMachine.CurrentStateType == PlayerState.Interacting);
        animator.SetBool(IsStunnedHash, stateMachine.CurrentStateType == PlayerState.Stunned);
        animator.SetBool(IsDeadHash, stateMachine.CurrentStateType == PlayerState.Dead);
    }

    private Vector2 GetCardinalDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.01f)
        {
            return lastFacingDirection;
        }

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0f)
            {
                return Vector2.right;
            }

            return Vector2.left;
        }

        if (direction.y > 0f)
        {
            return Vector2.up;
        }

        return Vector2.down;
    }

    private void HandleSpriteFlip()
    {
        spriteRenderer.flipX = false;
    }
}