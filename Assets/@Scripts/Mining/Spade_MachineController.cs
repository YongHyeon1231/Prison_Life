using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Spade_MachineController : MachineController
{
    [SerializeField] private PutDownTheRockPile _input;
    [SerializeField] private SupplyStack        _output;

    private static readonly int RUN = Animator.StringToHash("Run");

    protected override void Start()
    {
        _output.OnCountChanged += count =>
            GameManager.Instance.Resource.SetSpadeCount(count);

        base.Start();
    }

    // ── MachineController 구현 ────────────────────────────────────

    protected override bool CanProcess() => _input.ObjectCount > 0;

    protected override IEnumerator CoRunCycle()
    {
        MachineAnimator.SetTrigger(RUN);

        // Run 상태 진입 대기
        yield return null;
        while (MachineAnimator.IsInTransition(0) ||
               !MachineAnimator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
            yield return null;

        // Run 진입 직후 입력 광물 제거
        GameObject rock = _input.RemoveFromPile();
        if (rock != null)
        {
            DOTween.Kill(rock.transform);
            rock.transform.DOScale(Vector3.zero, 0.2f)
                .OnComplete(() => Destroy(rock));
        }

        // Run 애니메이션 완료 대기
        while (MachineAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.95f)
            yield return null;

        // 스페이드 출력
        _output.AddItem();
    }

    // ── 상태 후크 (선택적 확장) ────────────────────────────────────

    protected override void OnStateEnter(MachineState state)
    {
        if (state == MachineState.Running)
            GameManager.Instance.Sound.Play(Define.SoundType.MachineSound);
    }

    protected override void OnStateExit(MachineState state)
    {
        // 필요 시 정지 이펙트 등 추가
    }
}
