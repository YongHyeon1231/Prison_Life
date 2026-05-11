using UnityEngine;
using static Define;

public class UI_PlayerLevelUpArea : UI_FillProgressArea
{
    [Header("Level Up")]
    [SerializeField] private Animator _mineShopAnimator;
    [SerializeField] private int      _upgradedAmount = 50;

    [Header("Tutorial")]
    [SerializeField] private bool _completesTutorialStep;

    private bool _isUpgraded = false;

    protected override void OnComplete(PlayerController player)
    {
        if (!_isUpgraded)
        {
            _isUpgraded = true;
            if (_mineShopAnimator != null)
                _mineShopAnimator.Play(MINE_SHOP_FULL_AMOUNT);
            player.UpgradeWeaponLevel();
            ResetFill(_upgradedAmount);
        }
        else
        {
            player.UpgradeWeaponLevel();
            if (_completesTutorialStep && TutorialGuide.Instance != null)
                TutorialGuide.Instance.CompleteCurrentStep();
            gameObject.SetActive(false);
        }
    }
}
