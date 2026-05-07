public class PlayerDodgeState : PlayerBaseState
{
    public PlayerDodgeState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.PlayerMovement.StopMovement();
        stateMachine.PlayerDodge.Dodge(stateMachine.PlayerMovement.FacingDirection);
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
    }

    public override void Exit()
    {
    }
}