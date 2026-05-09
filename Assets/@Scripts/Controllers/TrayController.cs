using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 플레이어가 소지한 트레이 위에 아이템을 쌓는 컨트롤러.
/// - 아이템이 하나라도 있으면 MeshRenderer를 활성화합니다.
/// - 아이템이 모두 없어지면 MeshRenderer를 비활성화합니다.
/// - OnCountChanged 이벤트로 외부(InventoryManager 등)와 동기화합니다.
/// </summary>
[RequireComponent(typeof(MeshRenderer))]
public class TrayController : MonoBehaviour
{
    [Header("Stack")]
    [SerializeField] private float _itemHeight   = 0.5f;
    [SerializeField] private float _jumpPower    = 5f;
    [SerializeField] private float _jumpDuration = 0.3f;

    [Header("Bend")]
    [SerializeField] private Vector2 _shakeRange = new(0.8f, 0.4f);
    [SerializeField] private float   _bendFactor = 0.1f;

    public int  ItemCount  => _items.Count;
    public int  TotalCount => _reserved.Count + _items.Count;
    public bool HasItems   => TotalCount > 0;

    /// <summary>확정 아이템 수(예약 제외)가 바뀔 때 발행됩니다.</summary>
    public event Action<int> OnCountChanged;

    private MeshRenderer _meshRenderer;

    private readonly HashSet<Transform> _reserved = new();
    private readonly List<Transform>    _items    = new();

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        SetVisible(false);
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
            float rate = Mathf.Lerp(_shakeRange.x, _shakeRange.y, i / (float)_items.Count);

            Vector3    pos = _items[i - 1].position + _items[i - 1].up * _itemHeight;
            Quaternion rot = Quaternion.Lerp(_items[i].rotation, _items[i - 1].rotation, rate);

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

    /// <summary>
    /// 외부 오브젝트를 트레이 위로 DOJump 이동시켜 적재합니다.
    /// 첫 아이템 수신 시 트레이 Mesh를 자동으로 표시합니다.
    /// </summary>
    public void ReceiveItem(GameObject item)
    {
        SetVisible(true);

        Transform t = item.transform;
        _reserved.Add(t);

        Vector3 dest = transform.position + Vector3.up * TotalCount * _itemHeight;

        t.DOJump(dest, _jumpPower, 1, _jumpDuration)
            .OnComplete(() =>
            {
                _reserved.Remove(t);
                _items.Add(t);
                t.SetParent(transform);
                t.rotation = Quaternion.identity;
                OnCountChanged?.Invoke(ItemCount);
            });
    }

    /// <summary>트레이 최상단 아이템을 꺼내 반환합니다.</summary>
    public GameObject TakeItem()
    {
        if (_items.Count == 0) return null;

        Transform t = _items.Last();
        _items.RemoveAt(_items.Count - 1);
        t.SetParent(null);

        if (_reserved.Count == 0 && _items.Count == 0)
            SetVisible(false);

        OnCountChanged?.Invoke(ItemCount);
        return t.gameObject;
    }

    // ── 내부 ─────────────────────────────────────────────────

    private void SetVisible(bool visible)
    {
        if (_meshRenderer != null)
            _meshRenderer.enabled = visible;
    }
}
