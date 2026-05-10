using UnityEngine;
using static Define;

public class UI_PlayerLevelUpArea : UI_ConstructionArea
{
    [Header("Level Up")]
    [SerializeField] private Animator _mineShopAnimator;

    private bool _isUpgraded = false;

    protected override void OnComplete(PlayerController player)
    {
        if (!_isUpgraded)
        {
            _isUpgraded = true;

            if (_mineShopAnimator != null)
                _mineShopAnimator.Play(MINE_SHOP_FULL_AMOUNT);

            base.OnComplete(player); // 무기 업그레이드 + 필 리셋
        }
        // 두 번째 완료: base 미호출 → _isComplete = true, _progress = 1 유지
        // fillAmount가 가득 찬 채로 상호작용 완전 차단
    }
}
