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

    [Header("Inventory Stack")]
    [SerializeField] private PlayerInventoryStack _inventoryStack;
    [SerializeField] private PlayerInventoryStack _starStack;
    [SerializeField] private Vector3              _starStackRockOffset = new Vector3(0f, 1f, 0f);

    [Header("Tray")]
    [SerializeField] private TrayController _tray;

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

    // 트레이에 아이템이 하나라도 있으면 Serving 상태
    public bool IsServing => _tray != null && _tray.HasItems;

    public PlayerInventoryStack InventoryStack => _inventoryStack;
    public PlayerInventoryStack StarStack      => _starStack;
    public TrayController       Tray           => _tray;

    private bool    _isOnWorkGround         = false;
    private float   _mineTimer              = 0f;
    private float   _mineClipLength         = 0.5f;
    private Vector3 _starStackDefaultLocal;

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

        if (_starStack != null)
            _starStackDefaultLocal = _starStack.transform.localPosition;

        if (_inventoryStack != null)
            _inventoryStack.OnStackChanged += count =>
            {
                GameManager.Instance.Inventory.SetCount(_inventoryStack.ItemType, count);
                if (_starStack != null)
                    _starStack.transform.localPosition = count > 0
                        ? _starStackDefaultLocal + _starStackRockOffset
                        : _starStackDefaultLocal;
            };

        if (_tray != null)
            _tray.OnCountChanged += count =>
                GameManager.Instance.Inventory.SetCount(InventoryItemType.Spade, count);

        if (_starStack != null)
            _starStack.OnStackChanged += count =>
                GameManager.Instance.Inventory.SetCount(InventoryItemType.Star, count);
    }

    private void Update()
    {
        HandleMovement();
        UpdateState();
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

    private Vector3 GetMoveDir()
    {
        Vector2 joy = GameManager.Instance.JoystickDir;
        return (Quaternion.Euler(0, 45, 0) * new Vector3(joy.x, 0, joy.y)).normalized;
    }

    // ──────────────────────────────────────────────
    //  State Machine
    // ──────────────────────────────────────────────

    private void UpdateState()
    {
        _mineTimer -= Time.deltaTime;

        bool isMoving  = GetMoveDir() != Vector3.zero;
        bool isServing = IsServing;
        // 트레이에 아이템이 있으면 채굴 상태 진입 불가
        bool isMining  = _isOnWorkGround && _mineTimer > 0f && !isServing;

        State = (isMining, isServing, isMoving) switch
        {
            (true,  _,     true)  => EState.Move_Mine,
            (true,  _,     false) => EState.Mine,
            (_,     true,  true)  => EState.Serving_Move,
            (_,     true,  false) => EState.Serving_Idle,
            (_,     _,     true)  => EState.Move,
            _                     => EState.Idle,
        };
    }

    private void UpdateAnimation()
    {
        switch (State)
        {
            case EState.Idle:         _animator.CrossFade(IDLE,         0.1f);  break;
            case EState.Move:         _animator.CrossFade(MOVE,         0.05f); break;
            case EState.Serving_Idle: _animator.CrossFade(SERVING_IDLE, 0.1f);  break;
            case EState.Serving_Move: _animator.CrossFade(SERVING_MOVE, 0.05f); break;
            case EState.Mine:         _animator.CrossFade(MINE,         0.1f);  break;
            case EState.Move_Mine:    _animator.CrossFade(MOVE_MINE,    0.05f); break;
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
        // 트레이에 아이템이 있으면 채굴 불가
        if (IsServing) return;

        if (_inventoryStack != null) _inventoryStack.AddItem();
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
