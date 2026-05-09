using System.Collections;
using UnityEngine;

/// <summary>
/// 모든 기계(MachineController) 의 추상 베이스.
///
/// 서브클래스 구현 포인트
///   - CanProcess()  : 처리를 시작할 수 있는지 (입력 자원 유무 등)
///   - CoRunCycle()  : 처리 1회 코루틴 (애니메이션 대기 → 입출력)
///   - OnStateEnter / OnStateExit : 상태 전환 후크 (선택)
///   - ShouldRun()   : 기본은 CanProcess(). 플레이어 존재 조건이 필요하면 재정의.
/// </summary>
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
            zone.OnPlayerEntered += _ => IsPlayerPresent = true;
            zone.OnPlayerExited  += _ => IsPlayerPresent = false;
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

    // ── 전환 ─────────────────────────────────────────────────────

    protected void TransitionTo(MachineState newState)
    {
        if (CurrentState == newState) return;
        OnStateExit(CurrentState);
        CurrentState = newState;
        OnStateEnter(newState);
    }

    protected virtual void OnStateEnter(MachineState state) { }
    protected virtual void OnStateExit(MachineState state)  { }

    // ── 서브클래스 구현 포인트 ────────────────────────────────────

    /// <summary>
    /// 처리 루프 진입 조건.
    /// 기본: 입력 아이템이 1개 이상이면 Run.
    /// 플레이어 존재 여부도 필요한 기계는 IsPlayerPresent를 포함하도록 재정의하세요.
    /// </summary>
    protected virtual bool ShouldRun() => CanProcess();

    /// <summary>처리를 시작할 수 있는지 여부 (입력 자원 유무 등).</summary>
    protected abstract bool CanProcess();

    /// <summary>처리 1사이클 코루틴.</summary>
    protected abstract IEnumerator CoRunCycle();
}
