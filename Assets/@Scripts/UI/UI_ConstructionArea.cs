using UnityEngine;

public class UI_ConstructionArea : UI_FillProgressArea
{
    [Header("Upgrade")]
    [SerializeField] private int _upgradedAmount = 50;

    protected override void OnComplete(PlayerController player)
    {
        player.UpgradeWeaponLevel();
        ResetFill(_upgradedAmount);
    }
}
