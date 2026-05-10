using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 이동 방향에 따라 아이템 스택을 휘어지게 표현하는 공통 베이스.
/// TrayController, PlayerInventoryStack 이 이를 상속합니다.
/// </summary>
public abstract class AnimatedStackBase : MonoBehaviour
{
    [Header("Stack")]
    [SerializeField] protected float _itemHeight   = 0.4f;
    [SerializeField] protected float _jumpPower    = 5f;
    [SerializeField] protected float _jumpDuration = 0.3f;

    [Header("Bend")]
    [SerializeField] protected Vector2 _shakeRange = new(0.8f, 0.4f);
    [SerializeField] protected float   _bendFactor = 0.1f;

    public int ItemCount  => _items.Count;
    public int TotalCount => _items.Count + _reserved.Count;

    protected readonly List<Transform>    _items    = new();
    protected readonly HashSet<Transform> _reserved = new();

    protected virtual void Update()
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
}
