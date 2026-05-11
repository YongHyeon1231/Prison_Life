using DG.Tweening;
using System.Collections;
using UnityEngine;
using static Define;

public class Spade_MachineController : MachineController
{
    [SerializeField] private PutDownTheRockPile _input;
    [SerializeField] private SupplyStack        _output;

    protected override void Start()
    {
        _output.OnCountChanged += count =>
            GameManager.Instance.Resource.SetSpadeCount(count);

        base.Start();
    }

    protected override bool CanProcess() => _input.ObjectCount > 0;

    protected override IEnumerator CoRunCycle()
    {
        MachineAnimator.SetTrigger(SPADE_MACHINE_RUN);

        yield return null;
        while (MachineAnimator.IsInTransition(0) ||
               !MachineAnimator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
            yield return null;

        GameObject rock = _input.RemoveFromPile();
        if (rock != null)
        {
            DOTween.Kill(rock.transform);
            rock.transform.DOScale(Vector3.zero, 0.2f)
                .OnComplete(() => Destroy(rock));
        }

        while (MachineAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.95f)
            yield return null;

        _output.AddItem();
    }

    protected override void OnStateEnter(MachineState state)
    {
        if (state == MachineState.Running)
            GameManager.Instance.Sound.Play(Define.SoundType.MachineSound);
    }

    protected override void OnStateExit(MachineState state) { }
}
