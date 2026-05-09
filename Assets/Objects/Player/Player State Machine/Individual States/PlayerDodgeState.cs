public class PlayerDodgeState : PlayerBaseState
{
    private PlayerAnimator playerAnimator;

    public PlayerDodgeState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        playerAnimator = stateMachine.GetComponent<PlayerAnimator>();
    }

    public override void Enter()
    {
        UnityEngine.Vector2 dodgeDirection = stateMachine.PlayerMovement.MoveInput;

        if (dodgeDirection.sqrMagnitude < 0.01f)
        {
            dodgeDirection = stateMachine.PlayerMovement.FacingDirection;
        }

        stateMachine.PlayerMovement.StopMovement();
        stateMachine.PlayerDodge.BeginDodge(dodgeDirection);

        if (playerAnimator != null)
        {
            playerAnimator.PlayDodgeAnimation(dodgeDirection);
        }
    }

    public override void Tick()
    {
        if (stateMachine.PlayerDodge.IsDodging == true)
        {
            return;
        }

        stateMachine.ReturnToMovementState();
    }

    public override void FixedTick()
    {
        stateMachine.PlayerDodge.FixedTickDodge();
    }

    public override void Exit()
    {
        stateMachine.PlayerDodge.StopDodge();
    }
}