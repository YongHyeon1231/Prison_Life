using System;
using DG.Tweening;
using UnityEngine;

public class AnimatedPile : PileBase
{
    [Header("Item")]
    protected GameObject _itemPrefab;

    [Header("Animation")]
    [SerializeField] private float _jumpPower       = 4f;
    [SerializeField] private float _jumpDuration    = 0.3f;
    [SerializeField] private float _heightOffset    = 0f;
    [SerializeField] private float _scaleMultiplier = 1f;

    public event Action<int> OnCountChanged;

    public void AddItem()
    {
        if (_itemPrefab == null) return;

        GameObject go = Instantiate(_itemPrefab);
        AnimateTo(go, go.transform.localScale);
    }

    public void ReceiveItem(GameObject go)
    {
        AnimateTo(go, go.transform.localScale);
    }

    public override GameObject RemoveFromPile()
    {
        GameObject go = base.RemoveFromPile();
        if (go != null) OnCountChanged?.Invoke(ObjectCount);
        return go;
    }

    private void AnimateTo(GameObject go, Vector3 originalScale)
    {
        Vector3 dest = GetPositionAt(ObjectCount) + Vector3.up * _heightOffset;

        _objects.Push(go);
        go.transform.SetParent(transform);
        go.transform.position   = transform.position;
        go.transform.localScale = originalScale * _scaleMultiplier;

        go.transform.DOJump(dest, _jumpPower, 1, _jumpDuration)
            .OnComplete(() => go.transform.rotation = Quaternion.identity);

        OnCountChanged?.Invoke(ObjectCount);
    }
}
