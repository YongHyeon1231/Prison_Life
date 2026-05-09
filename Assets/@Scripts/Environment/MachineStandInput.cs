using UnityEngine;

/// <summary>
/// MachineStandInput 오브젝트에 부착합니다.
/// 하위 PlayerInteraction 트리거 영역에 플레이어가 진입하면,
/// 플레이어 등 뒤 RockPile에서 Rock을 하나씩 꺼내 PutDownTheRockPile에 쌓습니다.
/// </summary>
public class MachineStandInput : MonoBehaviour
{
    [SerializeField] private float _transferInterval = 0.2f;

    private PutDownTheRockPile _pile;

    private void Start()
    {
        _pile = GetComponentInChildren<PutDownTheRockPile>();

        // Rock이 쌓일 때마다 ResourceManager에 수량 동기화
        _pile.OnCountChanged += count =>
            GameManager.Instance.Resource.SetRockCount(count);

        PlayerInteraction interaction = GetComponentInChildren<PlayerInteraction>();
        interaction.InteractInterval    = _transferInterval;
        interaction.OnPlayerInteraction = OnTransferRock;
    }

    private void OnTransferRock(PlayerController player)
    {
        PlayerInventoryStack stack = player.InventoryStack;
        if (stack == null || stack.ItemCount == 0) return;

        GameObject item = stack.TakeItem();
        if (item == null) return;

        _pile.ReceiveItem(item);
    }
}
