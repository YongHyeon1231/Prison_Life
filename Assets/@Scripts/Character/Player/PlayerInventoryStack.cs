using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using static Define;

public class PlayerInventoryStack : AnimatedStackBase
{
    [SerializeField] private InventoryItemType _itemType = InventoryItemType.Rock;

    private GameObject _itemPrefab;
    private Transform  _container;

    [Header("Stack Limit")]
    [SerializeField] private bool _unlimited = false;
    [SerializeField] private int  _maxCount  = 20;

    public InventoryItemType ItemType => _itemType;
    public bool              IsFull   => !_unlimited && TotalCount >= _maxCount;

    public event Action<int> OnStackChanged;

    private void Start()
    {
        _itemPrefab = GameManager.Instance.Inventory.GetPrefab(_itemType);

        GameObject existingGO = GameObject.Find("##PlayerInventory");
        _container = existingGO != null
            ? existingGO.transform
            : new GameObject("##PlayerInventory").transform;
    }

    public void AddItem()
    {
        if (_itemPrefab == null || IsFull) return;

        Transform item = Instantiate(_itemPrefab).transform;
        item.SetParent(_container);
        _reserved.Add(item);

        Vector3 dest = transform.position + Vector3.up * TotalCount * _itemHeight;

        item.DOJump(dest, _jumpPower, 1, _jumpDuration)
            .OnComplete(() =>
            {
                _reserved.Remove(item);
                _items.Add(item);
                item.DOPunchScale(Vector3.one * 0.015f, 0.25f, 1, 0.5f);
                OnStackChanged?.Invoke(ItemCount);
            });
    }

    public void AddToStack(Transform item)
    {
        if (IsFull) { Destroy(item.gameObject); return; }

        item.SetParent(_container);
        _items.Add(item);
        item.DOPunchScale(Vector3.one * 0.015f, 0.25f, 1, 0.5f);
        OnStackChanged?.Invoke(ItemCount);
    }

    public GameObject TakeItem()
    {
        if (_items.Count == 0) return null;

        Transform item = _items.Last();
        _items.RemoveAt(_items.Count - 1);
        OnStackChanged?.Invoke(ItemCount);
        return item.gameObject;
    }

    public void Clear()
    {
        foreach (var item in _items)
            if (item != null) Destroy(item.gameObject);
        _items.Clear();
        OnStackChanged?.Invoke(0);
    }
}
