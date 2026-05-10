using UnityEngine;

public class UI_CampCapacityUpgradeArea : UI_ConstructionArea
{
    [SerializeField] private CampController _campController;

    private const int MaxCampCapacity = 30;

    protected override void OnComplete(PlayerController player)
    {
        _campController.Upgrade();
        ResetFill();

        if (_campController.MaxCapacity >= MaxCampCapacity)
            gameObject.SetActive(false);
    }
}
