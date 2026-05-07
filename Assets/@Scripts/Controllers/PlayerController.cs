using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum playerState
{
    Idle,
    Moving,
    Mining,
    Processing,
    Selling
}

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

            _animator.SetBool("isMoving", true);
        }
        else
        {
            _animator.SetBool("isMoving", false);
        }

        transform.position = new Vector3(transform.position.x, _groundY, transform.position.z);
    }
}
