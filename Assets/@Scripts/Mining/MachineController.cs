using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class MachineController : MonoBehaviour
{
    public enum MachineState { Idle, Running }

    public MachineState CurrentState { get; private set; } = MachineState.Idle;
    protected bool IsPlayerPresent { get; private set; }
    protected Animator MachineAnimator { get; private set; }

    protected virtual void Awake()
    {
        MachineAnimator = GetComponent<Animator>();

        PlayerInteraction zone = GetComponentInChildren<PlayerInteraction>();
        if (zone != null)
        {
            zone.OnEntered += _ => IsPlayerPresent = true;
            zone.OnExited  += _ => IsPlayerPresent = false;
        }
    }

    protected virtual void Start()
    {
        StartCoroutine(CoMachineLoop());
    }

    private IEnumerator CoMachineLoop()
    {
        while (true)
        {
            yield return new WaitUntil(ShouldRun);

            TransitionTo(MachineState.Running);
            yield return StartCoroutine(CoRunCycle());
            TransitionTo(MachineState.Idle);
        }
    }

    protected void TransitionTo(MachineState newState)
    {
        if (CurrentState == newState) return;
        OnStateExit(CurrentState);
        CurrentState = newState;
        OnStateEnter(newState);
    }

    protected virtual void OnStateEnter(MachineState state) { }
    protected virtual void OnStateExit(MachineState state)  { }

    protected virtual bool ShouldRun() => CanProcess();

    protected abstract bool CanProcess();

    protected abstract IEnumerator CoRunCycle();
}
