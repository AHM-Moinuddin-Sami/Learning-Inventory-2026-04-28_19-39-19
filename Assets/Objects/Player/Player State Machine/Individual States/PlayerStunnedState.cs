public class PlayerStunnedState : PlayerBaseState
{
    private float duration;
    private float timer;

    public PlayerStunnedState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public void SetDuration(float newDuration)
    {
        duration = newDuration;
    }

    public override void Enter()
    {
        timer = duration;
        stateMachine.PlayerMovement.StopMovement();
    }

    public override void Tick()
    {
        timer -= UnityEngine.Time.deltaTime;

        if (timer > 0f)
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