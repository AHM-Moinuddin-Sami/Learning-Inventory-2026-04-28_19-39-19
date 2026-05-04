using System.Collections;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private PlayerDodge playerDodge;
    [SerializeField] private PlayerInteraction playerInteraction;

    private PlayerState currentState = PlayerState.Idle;
    private Coroutine currentActionCoroutine;

    public PlayerState CurrentState
    {
        get
        {
            return currentState;
        }
    }

    public bool CanMove
    {
        get
        {
            return currentState == PlayerState.Idle || currentState == PlayerState.Moving;
        }
    }

    public bool CanAttack
    {
        get
        {
            return currentState == PlayerState.Idle || currentState == PlayerState.Moving;
        }
    }

    public bool CanDodge
    {
        get
        {
            return currentState == PlayerState.Idle || currentState == PlayerState.Moving;
        }
    }

    public bool CanInteract
    {
        get
        {
            return currentState == PlayerState.Idle || currentState == PlayerState.Moving;
        }
    }

    private void Reset()
    {
        inputReader = GetComponent<PlayerInputReader>();
        playerMovement = GetComponent<PlayerMovement>();
        playerCombat = GetComponent<PlayerCombat>();
        playerDodge = GetComponent<PlayerDodge>();
        playerInteraction = GetComponent<PlayerInteraction>();
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

    private void Update()
    {
        HandleMovementState();
    }

    private void FixedUpdate()
    {
        if (CanMove == false)
        {
            return;
        }

        playerMovement.TickMovement();
    }

    private void HandleMovementState()
    {
        if (CanMove == false)
        {
            return;
        }

        playerMovement.SetMoveInput(inputReader.MoveInput);

        if (inputReader.HasMovementInput == true)
        {
            ChangeState(PlayerState.Moving);
        }
        else
        {
            ChangeState(PlayerState.Idle);
        }
    }

    private void HandleAttackPressed()
    {
        if (CanAttack == false)
        {
            return;
        }

        StartStateAction(PlayerState.Attacking, AttackRoutine());
    }

    private void HandleDodgePressed()
    {
        if (CanDodge == false)
        {
            return;
        }

        StartStateAction(PlayerState.Dodging, DodgeRoutine());
    }

    private void HandleInteractPressed()
    {
        if (CanInteract == false)
        {
            return;
        }

        StartStateAction(PlayerState.Interacting, InteractRoutine());
    }

    private void StartStateAction(PlayerState newState, IEnumerator routine)
    {
        if (currentActionCoroutine != null)
        {
            StopCoroutine(currentActionCoroutine);
        }

        ChangeState(newState);
        playerMovement.StopMovement();

        currentActionCoroutine = StartCoroutine(routine);
    }

    private IEnumerator AttackRoutine()
    {
        playerCombat.Attack();

        while (currentState == PlayerState.Attacking)
        {
            yield return null;
        }
    }

    public void FinishAttack()
    {
        if (currentState != PlayerState.Attacking)
        {
            return;
        }

        currentActionCoroutine = null;
        ReturnToMovementState();
    }

    private IEnumerator DodgeRoutine()
    {
        playerDodge.Dodge(playerMovement.FacingDirection);

        yield return new WaitForSeconds(playerDodge.DodgeDuration);

        ReturnToMovementState();
    }

    private IEnumerator InteractRoutine()
    {
        playerInteraction.Interact(playerMovement.FacingDirection);

        yield return new WaitForSeconds(playerInteraction.InteractionDuration);

        ReturnToMovementState();
    }

    private void ReturnToMovementState()
    {
        currentActionCoroutine = null;

        if (inputReader.HasMovementInput == true)
        {
            ChangeState(PlayerState.Moving);
        }
        else
        {
            ChangeState(PlayerState.Idle);
        }
    }

    public void Stun(float duration)
    {
        StartStateAction(PlayerState.Stunned, StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        playerMovement.StopMovement();

        yield return new WaitForSeconds(duration);

        ReturnToMovementState();
    }

    public void Die()
    {
        if (currentActionCoroutine != null)
        {
            StopCoroutine(currentActionCoroutine);
            currentActionCoroutine = null;
        }

        playerMovement.StopMovement();
        ChangeState(PlayerState.Dead);
    }

    private void ChangeState(PlayerState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;
    }
}