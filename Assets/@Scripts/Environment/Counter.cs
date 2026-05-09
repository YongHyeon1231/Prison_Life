using UnityEngine;

/// <summary>
/// InfoDesk_Info의 PlayerInteraction이 플레이어를 감지하면
/// 플레이어 Tray에서 아이템을 꺼내 ItemPlace(TrayToItemPlacePile)로 이동시킵니다.
///
/// Inspector 연결
///   _itemPlace : Counter 하위 ItemPlace 오브젝트의 TrayToItemPlacePile
/// </summary>
public class Counter : MonoBehaviour
{
    [SerializeField] private TrayToItemPlacePile _itemPlace;

    private void Start()
    {
        PlayerInteraction desk =
            Utils.FindChild<PlayerInteraction>(gameObject, "InfoDesk_Input", recursive: true);

        if (desk == null)
        {
            Debug.LogWarning("[Counter] InfoDesk_Info에서 PlayerInteraction을 찾을 수 없습니다.");
            return;
        }

        desk.InteractInterval    = 0.1f;
        desk.OnPlayerInteraction = OnPlayerInteraction;
    }

    private void OnPlayerInteraction(PlayerController player)
    {
        TrayController tray = player.Tray;
        if (tray == null || !tray.HasItems) return;

        GameObject item = tray.TakeItem();
        if (item == null) return;

        _itemPlace.ReceiveItem(item);
    }
}
