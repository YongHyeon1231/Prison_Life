using System;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 프리팹을 주입받아 DOTween으로 격자에 쌓는 범용 파일.
/// - AddItem()      : 내부 프리팹을 스폰해 쌓기 (기계 출력 등)
/// - ReceiveItem()  : 외부에서 넘겨받은 오브젝트를 쌓기 (플레이어 전달 등)
/// - OnCountChanged : 개수 변화 구독 포인트
/// </summary>
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

    // ── 외부 API ─────────────────────────────────────────────

    /// <summary>내부 프리팹을 인스턴스화해 파일에 추가합니다.</summary>
    public void AddItem()
    {
        if (_itemPrefab == null) return;

        GameObject go = Instantiate(_itemPrefab);
        AnimateTo(go, go.transform.localScale);
    }

    /// <summary>외부에서 넘겨받은 오브젝트를 파일에 추가합니다.</summary>
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

    // ── 내부 ─────────────────────────────────────────────────

    private void AnimateTo(GameObject go, Vector3 originalScale)
    {
        Vector3 dest = GetPositionAt(ObjectCount) + Vector3.up * _heightOffset;

        _objects.Push(go);
        go.transform.SetParent(transform);
        go.transform.position   = transform.position;
        go.transform.localScale = originalScale * _scaleMultiplier; // 즉시 확정 크기 적용

        go.transform.DOJump(dest, _jumpPower, 1, _jumpDuration)
            .OnComplete(() => go.transform.rotation = Quaternion.identity);

        OnCountChanged?.Invoke(ObjectCount);
    }
}
