using UnityEngine;
using static Define;

/// <summary>
/// 쌓여 있는 아이템을 플레이어 트레이로 한 개씩 공급하는 스택.
/// 상위 오브젝트의 PlayerInteraction이 플레이어를 감지하면 일정 주기마다 한 개씩 지급합니다.
///
/// Inspector 설정:
///   - _dispenseInterval : 지급 주기(초)
///   - _itemPrefab, 기타 AnimatedPile 설정은 부모 클래스 Inspector에서 지정
/// </summary>
public class SupplyStack : AnimatedPile
{
    [Header("Dispense")]
    [SerializeField] private float _dispenseInterval    = 0.2f;
    [SerializeField] private float _dispenseJumpPower   = 6f;
    [SerializeField] private float _dispenseJumpDuration = 0.4f;

    private void Start()
    {
        _itemPrefab = GameManager.Instance.Inventory.GetPrefab(InventoryItemType.Spade);

        PlayerInteraction interaction = InteractionManager.Instance.SupplyPlayerZone;
        interaction.InteractInterval    = _dispenseInterval;
        interaction.OnInteraction = DispenseToPlayer;
    }

    // ── 내부 ─────────────────────────────────────────────────

    private void DispenseToPlayer(PlayerController player)
    {
        TrayController tray = player.Tray;
        if (tray == null) return;

        GameObject item = RemoveFromPile();
        if (item == null) return;

        tray.ReceiveItem(item, _dispenseJumpPower, _dispenseJumpDuration);
        GameManager.Instance.Sound.Play(Define.SoundType.GetOnSpade);
    }
}
