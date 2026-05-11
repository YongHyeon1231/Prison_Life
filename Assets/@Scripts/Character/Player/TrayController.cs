using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

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

    private void SetVisible(bool visible)
    {
        if (_meshRenderer != null)
            _meshRenderer.enabled = visible;
    }
}
