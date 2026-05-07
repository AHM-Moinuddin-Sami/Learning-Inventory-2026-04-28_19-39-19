using UnityEngine;

[RequireComponent(typeof(PlayerInputReader))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerCombat))]
[RequireComponent(typeof(PlayerDodge))]
[RequireComponent(typeof(PlayerInteraction))]
public class PlayerStateMachine : MonoBehaviour
{
    [SerializeField] private PlayerState currentStateType;

    private PlayerInputReader inputReader;
    private PlayerMovement playerMovement;
    private PlayerCombat playerCombat;
    private PlayerDodge playerDodge;
    private PlayerInteraction playerInteraction;

    private PlayerBaseState currentState;

    #region Individual states
    private PlayerIdleState idleState;
    private PlayerMoveState moveState;
    private PlayerAttackState attackState;
    private PlayerDodgeState dodgeState;
    private PlayerInteractState interactState;
    private PlayerStunnedState stunnedState;
    private PlayerDeadState deadState;
    #endregion

    #region Public state getter declarations
    public PlayerInputReader InputReader
    {
        get
        {
            return inputReader;
        }
    }

    public PlayerMovement PlayerMovement
    {
        get
        {
            return playerMovement;
        }
    }

    public PlayerCombat PlayerCombat
    {
        get
        {
            return playerCombat;
        }
    }

    public PlayerDodge PlayerDodge
    {
        get
        {
            return playerDodge;
        }
    }

    public PlayerInteraction PlayerInteraction
    {
        get
        {
            return playerInteraction;
        }
    }

    public PlayerState CurrentStateType
    {
        get
        {
            return currentStateType;
        }
    }

    #endregion

    private void Awake()
    {
        inputReader = GetComponent<PlayerInputReader>();
        playerMovement = GetComponent<PlayerMovement>();
        playerCombat = GetComponent<PlayerCombat>();
        playerDodge = GetComponent<PlayerDodge>();
        playerInteraction = GetComponent<PlayerInteraction>();

        idleState = new PlayerIdleState(this);
        moveState = new PlayerMoveState(this);
        attackState = new PlayerAttackState(this);
        dodgeState = new PlayerDodgeState(this);
        interactState = new PlayerInteractState(this);
        stunnedState = new PlayerStunnedState(this);
        deadState = new PlayerDeadState(this);
    }

    private void OnEnable()
    {
        inputReader.OnAttackPressed += HandleAttackPressed;
        inputReader.OnDodgePressed += HandleDodgePressed;
        inputReader.OnInteractPressed += HandleInteractPressed;
    }

    private void OnDisable()
    {
        inputReader.OnAttackPressed -= HandleAttackPressed;
        inputReader.OnDodgePressed -= HandleDodgePressed;
        inputReader.OnInteractPressed -= HandleInteractPressed;
    }

    private void Start()
    {
        SwitchState(idleState, PlayerState.Idle);
    }

    private void Update()
    {
        if (currentState == null)
        {
            return;
        }

        currentState.Tick();
    }

    private void FixedUpdate()
    {
        if (currentState == null)
        {
            return;
        }

        currentState.FixedTick();
    }

    public void SwitchState(PlayerBaseState newState, PlayerState newStateType)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;
        currentStateType = newStateType;

        currentState.Enter();
    }

    public void SwitchToIdleState()
    {
        SwitchState(idleState, PlayerState.Idle);
    }

    public void SwitchToMoveState()
    {
        SwitchState(moveState, PlayerState.Moving);
    }

    public void SwitchToAttackState()
    {
        SwitchState(attackState, PlayerState.Attacking);
    }

    public void SwitchToDodgeState()
    {
        SwitchState(dodgeState, PlayerState.Dodging);
    }

    public void SwitchToInteractState(IInteractable interactable)
    {
        interactState.SetInteractable(interactable);
        SwitchState(interactState, PlayerState.Interacting);
    }

    public void SwitchToStunnedState(float duration)
    {
        stunnedState.SetDuration(duration);
        SwitchState(stunnedState, PlayerState.Stunned);
    }

    public void SwitchToDeadState()
    {
        SwitchState(deadState, PlayerState.Dead);
    }

    public void ReturnToMovementState()
    {
        if (inputReader.HasMovementInput == true)
        {
            SwitchToMoveState();
        }
        else
        {
            SwitchToIdleState();
        }
    }

    private bool IsBusy()
    {
        return currentStateType == PlayerState.Dead ||
               currentStateType == PlayerState.Stunned ||
               currentStateType == PlayerState.Attacking ||
               currentStateType == PlayerState.Dodging ||
               currentStateType == PlayerState.Interacting;
    }

    private void HandleAttackPressed(AttackInputType inputType)
    {
        if (IsBusy())
        {
            return;
        }

        if (currentStateType == PlayerState.Attacking)
        {
            return;
        }

        bool startedAttack = playerCombat.TryStartAttack(inputType);

        if (startedAttack == false)
        {
            return;
        }

        SwitchToAttackState();
    }

    private void HandleDodgePressed()
    {
        if (IsBusy() == true)
        {
            return;
        }

        SwitchToDodgeState();
    }

    private void HandleInteractPressed()
    {
        if (currentStateType == PlayerState.Dead ||
            currentStateType == PlayerState.Stunned ||
            currentStateType == PlayerState.Attacking ||
            currentStateType == PlayerState.Dodging ||
            currentStateType == PlayerState.Interacting)
        {
            return;
        }

        IInteractable interactable;

        bool foundInteractable = playerInteraction.TryGetInteractable(
            playerMovement.FacingDirection,
            out interactable
        );

        if (foundInteractable == false)
        {
            return;
        }

        if (interactable.InteractionType == InteractionType.NonBlocking)
        {
            interactable.Interact();
            return;
        }

        SwitchToInteractState(interactable);
    }

    public void FinishAttack()
    {
        if (currentStateType != PlayerState.Attacking)
        {
            return;
        }

        ReturnToMovementState();
    }

    public void FinishInteraction()
    {
        if (currentStateType != PlayerState.Interacting)
        {
            return;
        }

        ReturnToMovementState();
    }
}