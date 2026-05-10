using UnityEngine;
using static Define;

public class UI_PlayerLevelUpArea : UI_ConstructionArea
{
    [Header("Level Up")]
    [SerializeField] private Animator _mineShopAnimator;

    private bool _isUpgraded = false;

    protected override void OnComplete(PlayerController player)
    {
        bool shouldFinalize = _isUpgraded;
        _isUpgraded = true;

        base.OnComplete(player);

        if (!shouldFinalize) return;

        if (_mineShopAnimator != null)
            _mineShopAnimator.Play(MINE_SHOP_FULL_AMOUNT);

        gameObject.SetActive(false);
    }
}
