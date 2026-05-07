public class PlayerMoveState : PlayerBaseState
{
    public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
    }

    public override void Tick()
    {
        if (stateMachine.InputReader.HasMovementInput == false)
        {
            stateMachine.SwitchToIdleState();
            return;
        }

        stateMachine.PlayerMovement.SetMoveInput(stateMachine.InputReader.MoveInput);
    }

    public override void FixedTick()
    {
        stateMachine.PlayerMovement.TickMovement();
    }

    public override void Exit()
    {
    }
}