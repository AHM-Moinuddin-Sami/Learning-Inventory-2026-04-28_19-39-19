public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.PlayerMovement.StopMovement();
    }

    public override void Tick()
    {
        if (stateMachine.InputReader.HasMovementInput == true)
        {
            stateMachine.SwitchToMoveState();
        }
    }

    public override void FixedTick()
    {
    }

    public override void Exit()
    {
    }
}