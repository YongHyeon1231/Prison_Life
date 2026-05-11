using TMPro;
using UnityEngine;
using static Define;

public class UI_StarText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    private void Start()
    {
        GameManager.Instance.Inventory.OnCountChanged += OnInventoryChanged;
        _text.text = GameManager.Instance.Inventory.GetCount(InventoryItemType.Star).ToString();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.Inventory.OnCountChanged -= OnInventoryChanged;
    }

    private void OnInventoryChanged(InventoryItemType type, int count)
    {
        if (type != InventoryItemType.Star) return;
        _text.text = count.ToString();
    }
}
