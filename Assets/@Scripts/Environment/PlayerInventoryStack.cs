using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 플레이어 등 뒤에 아이템을 시각적으로 쌓는 스택.
/// 아이템 종류(ItemType)만 선언하고 수량 관리는 InventoryManager에 위임합니다.
/// PlayerController가 OnStackChanged를 구독해 InventoryManager와 동기화합니다.
/// </summary>
public class PlayerInventoryStack : MonoBehaviour
{
    [SerializeField] private InventoryItemType _itemType = InventoryItemType.Rock;

    private GameObject _itemPrefab;

    [Header("Stack")]
    [SerializeField] private int   _maxCount     = 20;
    [SerializeField] private float _itemHeight   = 0.4f;
    [SerializeField] private float _jumpPower    = 4f;
    [SerializeField] private float _jumpDuration = 0.3f;

    [Header("Bend")]
    [SerializeField] private Vector2 _shakeRange = new(0.8f, 0.4f);
    [SerializeField] private float   _bendFactor = 0.15f;

    public InventoryItemType ItemType     => _itemType;
    public int               ItemCount    => _items.Count;
    public int               TotalCount   => _items.Count + _reserved.Count;
    public bool              IsFull       => TotalCount >= _maxCount;

    /// <summary>확정된 아이템 수(예약 제외)가 바뀔 때마다 발행됩니다.</summary>
    public event Action<int> OnStackChanged;

    private readonly List<Transform>    _items    = new();
    private readonly HashSet<Transform> _reserved = new();

    // ── 초기화 ───────────────────────────────────────────────

    private void Start()
    {
        _itemPrefab = GameManager.Instance.Inventory.GetPrefab(_itemType);
        if (_itemPrefab == null)
            Debug.LogWarning($"[PlayerInventoryStack] {_itemType} 프리팹이 GameManager에 등록되지 않았습니다.");
    }

    // ── Update : 스택 휨 ──────────────────────────────────────

    private void Update()
    {
        if (_items.Count == 0) return;

        Vector3 dir     = GameManager.Instance.JoystickDir;
        Vector3 moveDir = (Quaternion.Euler(0, 45, 0) * new Vector3(dir.x, 0, dir.y)).normalized;

        _items[0].SetPositionAndRotation(transform.position, transform.rotation);

        for (int i = 1; i < _items.Count; i++)
        {
            float rate      = Mathf.Lerp(_shakeRange.x, _shakeRange.y, i / (float)_items.Count);
            Vector3    pos  = _items[i - 1].position + _items[i - 1].up * _itemHeight;
            Quaternion rot  = Quaternion.Lerp(_items[i].rotation, _items[i - 1].rotation, rate);

            if (moveDir != Vector3.zero)
            {
                Vector3 axis = Vector3.Cross(moveDir, Vector3.up).normalized;
                rot = Quaternion.AngleAxis(i * _bendFactor * rate, axis) * rot;
            }

            _items[i].SetPositionAndRotation(
                Vector3.Lerp(_items[i].position, pos, rate),
                rot);
        }
    }

    // ── 공개 API ──────────────────────────────────────────────

    /// <summary>내부 프리팹을 스폰해 스택에 추가합니다.</summary>
    public void AddItem()
    {
        if (_itemPrefab == null || IsFull) return;

        Transform item = Instantiate(_itemPrefab).transform;
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

    /// <summary>외부에서 날아온 아이템을 애니메이션 없이 스택에 바로 편입합니다.</summary>
    public void AddToStack(Transform item)
    {
        if (IsFull) { Destroy(item.gameObject); return; }

        item.SetParent(transform);
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
