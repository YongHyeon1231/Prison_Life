using UnityEngine;
using static Define;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed   = 3.0f;
    [SerializeField] private float _rotateSpeed = 360f;
    [SerializeField] private float _groundY     = 0f;

    [Header("Weapons")]
    [SerializeField] private GameObject[] _weapons;

    [Header("Weapon Level (1~3)")]
    [SerializeField] [Range(1, 3)] private int _weaponLevel = 1;

    [Header("Rock Pile")]
    [SerializeField] private RockPile _rockPile;

    // ──────────────────────────────────────────────

    private Animator            _animator;
    private CharacterController _controller;
    private AudioSource         _audioSource;

    private EState _state = EState.None;
    public  EState State
    {
        get => _state;
        private set
        {
            if (_state == value) return;
            _state = value;
            UpdateAnimation();
        }
    }

    public bool IsServing { get; set; } = false;

    private bool  _isOnWorkGround = false;
    private float _mineTimer      = 0f;
    private float _mineClipLength = 0.5f;

    public int MinedRockCount => _rockPile != null ? _rockPile.ItemCount : 0;

    // ──────────────────────────────────────────────

    private void Awake()
    {
        _animator    = GetComponent<Animator>();
        _controller  = GetComponent<CharacterController>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        SetWeaponsActive(false);

        foreach (var clip in _animator.runtimeAnimatorController.animationClips)
        {
            if (Animator.StringToHash(clip.name) == MINE)
            {
                _mineClipLength = clip.length;
                break;
            }
        }
    }

    private void Update()
    {
        HandleMovement();
        UpdateState();
    }

    // ──────────────────────────────────────────────
    //  Helpers
    // ──────────────────────────────────────────────

    private Vector3 GetMoveDir()
    {
        Vector2 joy = GameManager.Instance.JoystickDir;
        Vector3 dir = new(joy.x, 0, joy.y);
        return (Quaternion.Euler(0, 45, 0) * dir).normalized;
    }

    // ──────────────────────────────────────────────
    //  Movement
    // ──────────────────────────────────────────────

    private void HandleMovement()
    {
        Vector3 moveDir = GetMoveDir();

        if (moveDir != Vector3.zero)
        {
            _controller.Move(moveDir * _moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.LookRotation(moveDir),
                _rotateSpeed * Time.deltaTime);
        }

        transform.position = new Vector3(transform.position.x, _groundY, transform.position.z);
    }

    // ──────────────────────────────────────────────
    //  State Machine
    // ──────────────────────────────────────────────

    private void UpdateState()
    {
        bool isMoving = GetMoveDir() != Vector3.zero;
        bool isMining = _isOnWorkGround && _mineTimer > 0f;

        _mineTimer -= Time.deltaTime;

        State = (isMining, isMoving) switch
        {
            (true,  true)  => EState.Move_Mine,
            (true,  false) => EState.Mine,
            (false, true)  => EState.Move,
            _              => EState.Idle,
        };
    }

    private void UpdateAnimation()
    {
        switch (State)
        {
            case EState.Idle:
                _animator.CrossFade(IsServing ? SERVING_IDLE : IDLE, 0.1f);
                break;
            case EState.Move:
                _animator.CrossFade(IsServing ? SERVING_MOVE : MOVE, 0.05f);
                break;
            case EState.Mine:
                _animator.CrossFade(MINE, 0.1f);
                break;
            case EState.Move_Mine:
                _animator.CrossFade(MOVE_MINE, 0.05f);
                break;
        }
    }

    // ──────────────────────────────────────────────
    //  WorkGround
    // ──────────────────────────────────────────────

    public void EnterWorkGround()
    {
        _isOnWorkGround = true;
        EquipWeapon(_weaponLevel);
    }

    public void ExitWorkGround()
    {
        _isOnWorkGround = false;
        _mineTimer      = 0f;
        SetWeaponsActive(false);
    }

    // ──────────────────────────────────────────────
    //  Mining
    // ──────────────────────────────────────────────

    public void OnRockMined()
    {
        if (_rockPile != null) _rockPile.AddRock();

        _mineTimer = _mineClipLength;

        bool isMoving   = GetMoveDir() != Vector3.zero;
        int  targetHash = isMoving ? MOVE_MINE : MINE;
        _animator.CrossFade(targetHash, 0.05f, 0, 0f);
        _state = isMoving ? EState.Move_Mine : EState.Mine;
    }

    // ──────────────────────────────────────────────
    //  Weapon Management
    // ──────────────────────────────────────────────

    private void EquipWeapon(int level)
    {
        SetWeaponsActive(false);
        int idx = level - 1;
        if (_weapons != null && idx >= 0 && idx < _weapons.Length && _weapons[idx] != null)
            _weapons[idx].SetActive(true);
    }

    private void SetWeaponsActive(bool active)
    {
        if (_weapons == null) return;
        foreach (var w in _weapons)
            if (w != null) w.SetActive(active);
    }

    public void SetWeaponLevel(int level)
    {
        _weaponLevel = Mathf.Clamp(level, 1, 3);
        if (_isOnWorkGround)
            EquipWeapon(_weaponLevel);
    }
}
