using UnityEngine;

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
