using System.Collections;
using UnityEngine;

public class UI_CampCapacityUpgradeArea : UI_ConstructionArea
{
    [SerializeField] private CampController _campController;
    [SerializeField] private float          _hideDelay = 1f;

    protected override void OnComplete(PlayerController player)
    {
        _campController.Upgrade();
        StartCoroutine(CoHideAfterDelay());
    }

    private IEnumerator CoHideAfterDelay()
    {
        yield return new WaitForSeconds(_hideDelay);
        gameObject.SetActive(false);
    }
}
