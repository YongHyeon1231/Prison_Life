using UnityEngine;

public class MachineStandInput : MonoBehaviour
{
    [SerializeField] private float _transferInterval = 0.2f;

    private PutDownTheRockPile _pile;

    private void Start()
    {
        _pile = GetComponentInChildren<PutDownTheRockPile>();

        _pile.OnCountChanged += count =>
            GameManager.Instance.Resource.SetRockCount(count);

        PlayerInteraction interaction = GetComponentInChildren<PlayerInteraction>();
        interaction.InteractInterval  = _transferInterval;
        interaction.OnInteraction     = OnTransferRock;
    }

    private void OnTransferRock(PlayerController player)
    {
        PlayerInventoryStack stack = player.InventoryStack;
        if (stack == null || stack.ItemCount == 0) return;

        GameObject item = stack.TakeItem();
        if (item == null) return;

        _pile.ReceiveItem(item);
        GameManager.Instance.Sound.Play(Define.SoundType.PotDownRock);
    }
}
