using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerStateMachine))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerAnimator : MonoBehaviour
{
    // private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    // private static readonly int MoveYHash = Animator.StringToHash("MoveY");
    // private static readonly int FacingXHash = Animator.StringToHash("FacingX");
    // private static readonly int FacingYHash = Animator.StringToHash("FacingY");
    // private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    // private static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
    // private static readonly int IsDodgingHash = Animator.StringToHash("IsDodging");
    // private static readonly int IsInteractingHash = Animator.StringToHash("IsInteracting");
    // private static readonly int IsStunnedHash = Animator.StringToHash("IsStunned");
    // private static readonly int IsDeadHash = Animator.StringToHash("IsDead");

    private PlayerMovement playerMovement;
    private PlayerStateMachine stateMachine;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        stateMachine = GetComponent<PlayerStateMachine>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Vector2 moveInput = playerMovement.MoveInput;
        Vector2 facingDirection = playerMovement.FacingDirection;

        // animator.SetFloat(MoveXHash, moveInput.x);
        // animator.SetFloat(MoveYHash, moveInput.y);

        // animator.SetFloat(FacingXHash, facingDirection.x);
        // animator.SetFloat(FacingYHash, facingDirection.y);

        // animator.SetBool(IsMovingHash, playerMovement.IsMoving);

        // animator.SetBool(IsAttackingHash, stateMachine.CurrentStateType == PlayerState.Attacking);
        // animator.SetBool(IsDodgingHash, stateMachine.CurrentStateType == PlayerState.Dodging);
        // animator.SetBool(IsInteractingHash, stateMachine.CurrentStateType == PlayerState.Interacting);
        // animator.SetBool(IsStunnedHash, stateMachine.CurrentStateType == PlayerState.Stunned);
        // animator.SetBool(IsDeadHash, stateMachine.CurrentStateType == PlayerState.Dead);

        HandleSpriteFlip(facingDirection);
    }

    private void HandleSpriteFlip(Vector2 facingDirection)
    {
        if (facingDirection.x > 0.01f)
        {
            spriteRenderer.flipX = false;
        }
        else if (facingDirection.x < -0.01f)
        {
            spriteRenderer.flipX = true;
        }
    }
}