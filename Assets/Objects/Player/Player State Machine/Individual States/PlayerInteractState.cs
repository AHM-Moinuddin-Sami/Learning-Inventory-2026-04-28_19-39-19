public class PlayerInteractState : PlayerBaseState
{
    private IInteractable currentInteractable;
    private float timer;

    public PlayerInteractState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public void SetInteractable(IInteractable interactable)
    {
        currentInteractable = interactable;
    }

    public override void Enter()
    {
        stateMachine.PlayerMovement.StopMovement();

        if (currentInteractable == null)
        {
            stateMachine.ReturnToMovementState();
            return;
        }

        timer = currentInteractable.InteractionDuration;
        currentInteractable.Interact();
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
        currentInteractable = null;
    }
}