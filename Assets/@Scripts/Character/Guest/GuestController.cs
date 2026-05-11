using UnityEngine;
using UnityEngine.AI;
using static Define;

[RequireComponent(typeof(NavMeshAgent))]
public class GuestController : BaseCharacterController
{
    [SerializeField] private GameObject _guestSpade;
    [SerializeField] private GameObject _noSpaceBillboard;

    private NavMeshAgent   _agent;
    private UI_OrderBubble _orderBubble;
    private Quaternion?    _arrivalFacing = null;

    private bool _hasDestination = false;

    private EGuestState _state = EGuestState.None;
    public  EGuestState State
    {
        get => _state;
        private set
        {
            if (_state == value) return;
            _state = value;
            UpdateAnimation();
        }
    }

    public int  CurrentQueueIndex { get; set; }
    public int  RequiredCount     { get; private set; }

    public bool HasArrived =>
        _hasDestination &&
        _agent.isOnNavMesh &&
        !_agent.pathPending &&
        _agent.remainingDistance < 0.1f;

    protected override void Awake()
    {
        base.Awake();
        _agent       = GetComponent<NavMeshAgent>();
        _orderBubble = GetComponentInChildren<UI_OrderBubble>(true);

        _agent.speed                 = _moveSpeed;
        _agent.stoppingDistance      = 0.05f;
        _agent.radius                = 0.3f;
        _agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;

        if (_orderBubble)       _orderBubble.gameObject.SetActive(false);
        if (_guestSpade)        _guestSpade.SetActive(false);
        if (_noSpaceBillboard)  _noSpaceBillboard.SetActive(false);
    }

    private void Update()
    {
        if (HasArrived)
        {
            _agent.isStopped = true;
            State = EGuestState.Idle;

            if (_arrivalFacing.HasValue)
            {
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, _arrivalFacing.Value, _rotateSpeed * Time.deltaTime);
            }
        }
        else
        {
            LookAtDestination();
            State = EGuestState.Move;
        }
    }

    protected override void UpdateAnimation()
    {
        switch (State)
        {
            case EGuestState.Idle: _animator.CrossFade(GUEST_IDLE, 0.1f);  break;
            case EGuestState.Move: _animator.CrossFade(GUEST_MOVE, 0.05f); break;
        }
    }

    public void WalkToWaitPoint(Vector3 pos, Quaternion facing)
    {
        _arrivalFacing = facing;
        SetDestination(pos);
    }

    public void SetDestination(Vector3 pos)
    {
        _hasDestination = true;
        if (!_agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
                _agent.Warp(hit.position);
            else
                return;
        }
        _agent.isStopped = false;
        _agent.SetDestination(pos);
    }

    public void ShowOrder()
    {
        RequiredCount = GetWeightedRandom();
        if (_orderBubble)
        {
            _orderBubble.SetCount(RequiredCount);
            _orderBubble.gameObject.SetActive(true);
        }
    }

    public void OnServed()
    {
        if (_orderBubble) _orderBubble.gameObject.SetActive(false);
        if (_guestSpade)  _guestSpade.SetActive(true);
        GameManager.Instance.Sound.Play(SoundType.GuestGetItem);
        RequiredCount = 0;
    }

    public void ShowNoSpace() { if (_noSpaceBillboard) _noSpaceBillboard.SetActive(true); }
    public void HideNoSpace() { if (_noSpaceBillboard) _noSpaceBillboard.SetActive(false); }

    private int GetWeightedRandom()
    {
        int r = Random.Range(0, 100);
        if (r < 30) return 1;
        if (r < 55) return 2;
        if (r < 80) return 3;
        if (r < 92) return 4;
        return 5;
    }

    private void LookAtDestination()
    {
        Vector3 dir = (_agent.destination - transform.position).normalized;
        if (dir == Vector3.zero) return;
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.LookRotation(dir),
            _rotateSpeed * Time.deltaTime);
    }
}
