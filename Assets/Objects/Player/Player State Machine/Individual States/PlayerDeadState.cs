public class PlayerDeadState : PlayerBaseState
{
    public PlayerDeadState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.PlayerMovement.StopMovement();
    }

    public override void Tick()
    {
    }

    public override void FixedTick()
    {
    }

    public override void Exit()
    {
    }
}