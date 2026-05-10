using UnityEngine;

/// <summary>
/// 플레이어·게스트 등 인간형 캐릭터의 공통 베이스.
/// 이동 속도, 회전 속도, Animator 컴포넌트 참조를 통합합니다.
/// </summary>
[RequireComponent(typeof(Animator))]
public abstract class BaseCharacterController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] protected float _moveSpeed   = 3.0f;
    [SerializeField] protected float _rotateSpeed = 360f;

    protected Animator _animator;

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    protected abstract void UpdateAnimation();
}
