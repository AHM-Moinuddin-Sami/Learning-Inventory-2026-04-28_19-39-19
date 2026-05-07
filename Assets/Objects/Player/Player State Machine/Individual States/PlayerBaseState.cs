public abstract class PlayerBaseState
{
    protected PlayerStateMachine stateMachine;

    public PlayerBaseState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public abstract void Enter();

    public abstract void Tick();

    public abstract void FixedTick();

    public abstract void Exit();
}