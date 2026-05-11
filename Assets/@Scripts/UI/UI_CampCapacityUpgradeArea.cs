using System.Collections;
using UnityEngine;
using static Define;

public class UI_CampCapacityUpgradeArea : UI_FillProgressArea
{
    [SerializeField] private CampController _campController;
    [SerializeField] private float          _hideDelay = 1f;

    [Header("Tutorial")]
    [SerializeField] private bool _completesTutorialStep;

    protected override void OnComplete(PlayerController player)
    {
        GameManager.Instance.Sound.Play(SoundType.OpenAD);
        _campController.Upgrade();
        if (_completesTutorialStep && TutorialGuide.Instance != null)
            TutorialGuide.Instance.CompleteCurrentStep();
        StartCoroutine(CoHideAfterDelay());
    }

    private IEnumerator CoHideAfterDelay()
    {
        yield return new WaitForSeconds(_hideDelay);
        gameObject.SetActive(false);
    }
}
