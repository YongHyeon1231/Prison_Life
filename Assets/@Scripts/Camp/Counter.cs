using UnityEngine;

public class Counter : MonoBehaviour
{
    [SerializeField] private TrayToItemPlacePile _itemPlace;

    private void Start()
    {
        PlayerInteraction desk = InteractionManager.Instance.InfoDeskPlayerZone;
        if (desk == null) return;

        desk.InteractInterval    = 0.1f;
        desk.OnInteraction = OnPlayerInteraction;
    }

    private void OnPlayerInteraction(PlayerController player)
    {
        TrayController tray = player.Tray;
        if (tray == null || !tray.HasItems) return;

        GameObject item = tray.TakeItem();
        if (item == null) return;

        _itemPlace.ReceiveItem(item);
        GameManager.Instance.Sound.Play(Define.SoundType.ItemPutDown);
    }
}
