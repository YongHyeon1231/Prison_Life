using System;
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
public class TrayController : AnimatedStackBase
{
    public bool HasItems => TotalCount > 0;

    public event Action<int> OnCountChanged;

    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        SetVisible(false);
    }

    // ── 공개 API ──────────────────────────────────────────────

    /// <summary>
    /// 외부 오브젝트를 트레이 위로 DOJump 이동시켜 적재합니다.
    /// jumpPower/jumpDuration을 0으로 전달하면 Inspector의 기본값을 사용합니다.
    /// </summary>
    public void ReceiveItem(GameObject item, float jumpPower = 0f, float jumpDuration = 0f)
    {
        SetVisible(true);

        Transform t = item.transform;
        _reserved.Add(t);

        float power    = jumpPower    > 0f ? jumpPower    : _jumpPower;
        float duration = jumpDuration > 0f ? jumpDuration : _jumpDuration;

        Vector3 dest = transform.position + Vector3.up * TotalCount * _itemHeight;

        t.DOJump(dest, power, 1, duration)
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
