using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 플레이어 등 뒤에 아이템을 시각적으로 쌓는 스택.
/// 아이템 종류(ItemType)만 선언하고 수량 관리는 InventoryManager에 위임합니다.
/// PlayerController가 OnStackChanged를 구독해 InventoryManager와 동기화합니다.
/// </summary>
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

    /// <summary>확정된 아이템 수(예약 제외)가 바뀔 때마다 발행됩니다.</summary>
    public event Action<int> OnStackChanged;

    // ── 초기화 ───────────────────────────────────────────────

    private void Start()
    {
        _itemPrefab = GameManager.Instance.Inventory.GetPrefab(_itemType);
        if (_itemPrefab == null)
            Debug.LogWarning($"[PlayerInventoryStack] {_itemType} 프리팹이 GameManager에 등록되지 않았습니다.");

        // Rock·Star 아이템을 씬 루트 ##PlayerInventory 하위로 모아 Hierarchy를 정리합니다.
        GameObject existingGO = GameObject.Find("##PlayerInventory");
        _container = existingGO != null
            ? existingGO.transform
            : new GameObject("##PlayerInventory").transform;
    }

    // ── 공개 API ──────────────────────────────────────────────

    /// <summary>내부 프리팹을 스폰해 스택에 추가합니다.</summary>
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

    /// <summary>외부에서 날아온 아이템을 스택에 바로 편입합니다.</summary>
    public void AddToStack(Transform item)
    {
        if (IsFull) { Destroy(item.gameObject); return; }

        item.SetParent(_container);
        _items.Add(item);
        item.DOPunchScale(Vector3.one * 0.015f, 0.25f, 1, 0.5f);
        OnStackChanged?.Invoke(ItemCount);
    }

    /// <summary>스택 최상단 아이템을 꺼내 반환합니다.</summary>
    public GameObject TakeItem()
    {
        if (_items.Count == 0) return null;

        Transform item = _items.Last();
        _items.RemoveAt(_items.Count - 1);
        OnStackChanged?.Invoke(ItemCount);
        return item.gameObject;
    }

    /// <summary>스택을 전부 비웁니다.</summary>
    public void Clear()
    {
        foreach (var item in _items)
            if (item != null) Destroy(item.gameObject);
        _items.Clear();
        OnStackChanged?.Invoke(0);
    }
}
