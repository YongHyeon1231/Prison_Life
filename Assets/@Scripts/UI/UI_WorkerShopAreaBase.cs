using System.Collections;
using UnityEngine;
using static Define;

public abstract class UI_WorkerShopAreaBase : UI_FillProgressArea
{
    [Header("Worker Spawn")]
    [SerializeField] protected WorkerType _workerType;

    [Header("Tutorial")]
    [SerializeField] protected bool _completesTutorialStep;

    protected override void OnComplete(PlayerController player)
    {
        SpawnWorkers();
        if (_completesTutorialStep && TutorialGuide.Instance != null)
            TutorialGuide.Instance.CompleteCurrentStep();
        StartCoroutine(CoDeactivateAfterDelay(0.5f));
    }

    protected abstract void SpawnWorkers();

    private IEnumerator CoDeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        OnBeforeDeactivate();
        gameObject.SetActive(false);
    }

    protected virtual void OnBeforeDeactivate() { }
}
