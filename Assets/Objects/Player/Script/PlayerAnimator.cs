using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerStateMachine stateMachine;
    [SerializeField] private Animator animator;

    private void Reset()
    {
        playerMovement = GetComponent<PlayerMovement>();
        stateMachine = GetComponent<PlayerStateMachine>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Vector2 moveInput = playerMovement.MoveInput;
        Vector2 facingDirection = playerMovement.FacingDirection;

        animator.SetFloat("MoveX", moveInput.x);
        animator.SetFloat("MoveY", moveInput.y);

        animator.SetFloat("FacingX", facingDirection.x);
        animator.SetFloat("FacingY", facingDirection.y);

        animator.SetBool("IsMoving", stateMachine.CurrentState == PlayerState.Moving);

        animator.SetBool("IsAttacking", stateMachine.CurrentState == PlayerState.Attacking);
        animator.SetBool("IsDodging", stateMachine.CurrentState == PlayerState.Dodging);
        animator.SetBool("IsInteracting", stateMachine.CurrentState == PlayerState.Interacting);
        animator.SetBool("IsStunned", stateMachine.CurrentState == PlayerState.Stunned);
        animator.SetBool("IsDead", stateMachine.CurrentState == PlayerState.Dead);
    }
}