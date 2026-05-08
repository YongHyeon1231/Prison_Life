using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 플레이어가 채굴한 Rock을 등 뒤에 수직으로 쌓습니다.
/// TrayController와 동일한 스택 + 이동 휨 방식으로 동작합니다.
/// </summary>
public class RockPile : MonoBehaviour
{
    [SerializeField] private GameObject _rockPrefab;

    [Header("Stack")]
    [SerializeField] private int    _maxCount     = 20;
    [SerializeField] private float  _itemHeight   = 0.4f;
    [SerializeField] private float  _jumpPower    = 4f;
    [SerializeField] private float  _jumpDuration = 0.3f;

    [Header("Bend")]
    [SerializeField] private Vector2 _shakeRange = new(0.8f, 0.4f);
    [SerializeField] private float   _bendFactor = 0.15f;

    public int ItemCount     => _items.Count;
    public int ReservedCount => _reserved.Count;
    public int TotalCount    => _items.Count + _reserved.Count;

    private readonly List<Transform>    _items    = new();
    private readonly HashSet<Transform> _reserved = new();

    private void Update()
    {
        if (_items.Count == 0) return;

        Vector3 dir     = GameManager.Instance.JoystickDir;
        Vector3 moveDir = new Vector3(dir.x, 0, dir.y);
        moveDir = (Quaternion.Euler(0, 45, 0) * moveDir).normalized;

        _items[0].SetPositionAndRotation(transform.position, transform.rotation);

        for (int i = 1; i < _items.Count; i++)
        {
            float rate = Mathf.Lerp(_shakeRange.x, _shakeRange.y, i / (float)_items.Count);

            Vector3    targetPos = _items[i - 1].position + _items[i - 1].up * _itemHeight;
            Quaternion targetRot = Quaternion.Lerp(_items[i].rotation, _items[i - 1].rotation, rate);

            if (moveDir != Vector3.zero)
            {
                Vector3 worldTiltAxis = Vector3.Cross(moveDir, Vector3.up).normalized;
                float   bendAngle     = i * _bendFactor * rate;
                targetRot = Quaternion.AngleAxis(bendAngle, worldTiltAxis) * targetRot;
            }

            _items[i].SetPositionAndRotation(
                Vector3.Lerp(_items[i].position, targetPos, rate),
                targetRot);
        }
    }

    public bool IsFull => TotalCount >= _maxCount;

    public void AddRock()
    {
        if (_rockPrefab == null) return;
        if (IsFull) return;

        Transform item = Instantiate(_rockPrefab).transform;
        _reserved.Add(item);

        Vector3 dest = transform.position + Vector3.up * TotalCount * _itemHeight;

        item.DOJump(dest, _jumpPower, 1, _jumpDuration)
            .OnComplete(() =>
            {
                _reserved.Remove(item);
                _items.Add(item);
            });
    }

    public GameObject TakeRock()
    {
        if (_items.Count == 0) return null;

        Transform item = _items.Last();
        _items.RemoveAt(_items.Count - 1);
        return item.gameObject;
    }

    public void Clear()
    {
        foreach (var item in _items)
            if (item != null) Destroy(item.gameObject);
        _items.Clear();
    }
}
