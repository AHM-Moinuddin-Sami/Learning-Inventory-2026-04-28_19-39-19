public class PlayerAttackState : PlayerBaseState
{
    private float attackTimer;

    public PlayerAttackState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        attackTimer = stateMachine.PlayerCombat.CurrentAttackLockDuration;
        stateMachine.PlayerMovement.StopMovement();
    }

    public override void Tick()
    {
        attackTimer -= UnityEngine.Time.deltaTime;

        if (attackTimer > 0f)
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
        stateMachine.PlayerCombat.ClearCurrentAttack();
    }
}