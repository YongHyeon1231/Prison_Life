using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 3.0f;

    [SerializeField] private float _rotateSpeed = 360;

    [SerializeField] private float _groundY = 0f;

    private Animator _animator;
    private CharacterController _controller;
    private AudioSource _audioSource;

    private EState _state = EState.None;
    public EState State
    {
        get {return _state; }
        private set
        {
            if (_state == value) return;

            _state = value;
            UpdateAnimation();
        }
    }

    public bool IsServing { get; set; } = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        Vector3 dir = GameManager.Instance.JoystickDir;
        Vector3 moveDir = new Vector3(dir.x, 0, dir.y);
        moveDir = (Quaternion.Euler(0, 45, 0) * moveDir).normalized;

        if (moveDir != Vector3.zero)
        {
            _controller.Move(moveDir * _moveSpeed * Time.deltaTime);

            Quaternion lookRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, _rotateSpeed * Time.deltaTime);

            State = EState.Move;
        }
        else
        {
            State = EState.Idle;
        }

        transform.position = new Vector3(transform.position.x, _groundY, transform.position.z);
    }

    private void UpdateAnimation()
    {
        switch (State)
        {
            case EState.Idle:
                _animator.CrossFade(IsServing ? Define.SERVING_IDLE : Define.IDLE, 0.1f);
                break;
            case EState.Move:
                _animator.CrossFade(IsServing ? Define.SERVING_MOVE : Define.MOVE, 0.05f);
                break;
        }
    }
}
