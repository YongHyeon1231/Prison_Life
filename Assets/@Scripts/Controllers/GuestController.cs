using UnityEngine;
using UnityEngine.AI;
using static Define;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class GuestController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed   = 2f;
    [SerializeField] private float _rotateSpeed = 360f;

    private Animator       _animator;
    private NavMeshAgent   _agent;
    private UI_OrderBubble _orderBubble;
    private GameObject     _guestSpade;

    private bool _hasDestination = false;

    // ── 상태머신 ──────────────────────────────────────────────

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

    // ── 공개 상태 ──────────────────────────────────────────────

    public int  CurrentQueueIndex { get; set; }
    public int  RequiredCount     { get; private set; }

    public bool HasArrived =>
        _hasDestination &&
        _agent.isOnNavMesh &&
        !_agent.pathPending &&
        _agent.remainingDistance < 0.1f;

    // ── 초기화 ────────────────────────────────────────────────

    private void Awake()
    {
        _animator    = GetComponent<Animator>();
        _agent       = GetComponent<NavMeshAgent>();
        _orderBubble = GetComponentInChildren<UI_OrderBubble>(true);
        _guestSpade  = Utils.FindChild(gameObject, "GuestSpade", recursive: true);

        _agent.speed                 = _moveSpeed;
        _agent.stoppingDistance      = 0.05f;
        _agent.radius                = 0.1f;
        _agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

        if (_orderBubble) _orderBubble.gameObject.SetActive(false);
        if (_guestSpade)  _guestSpade.SetActive(false);
    }

    // ── Update ────────────────────────────────────────────────

    private void Update()
    {
        if (HasArrived)
        {
            _agent.isStopped = true;
            State = EGuestState.Idle;
        }
        else
        {
            LookAtDestination();
            State = EGuestState.Move;
        }

        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
    }

    // ── 애니메이션 ────────────────────────────────────────────

    private void UpdateAnimation()
    {
        switch (State)
        {
            case EGuestState.Idle: _animator.CrossFade(GUEST_IDLE, 0.1f);  break;
            case EGuestState.Move: _animator.CrossFade(GUEST_MOVE, 0.05f); break;
        }
    }

    // ── 공개 API ──────────────────────────────────────────────

    public void SetDestination(Vector3 pos)
    {
        _hasDestination  = true;
        _agent.isStopped = false;
        _agent.SetDestination(pos);
    }

    /// <summary>대기열 맨 앞 도착 시 GuestManager가 호출 — 주문 수를 표시합니다.</summary>
    public void ShowOrder()
    {
        RequiredCount = GetWeightedRandom();
        if (_orderBubble)
        {
            _orderBubble.SetCount(RequiredCount);
            _orderBubble.gameObject.SetActive(true);
        }
    }

    /// <summary>서빙 완료 시 GuestManager가 호출 — GuestSpade 활성화, 말풍선 숨김.</summary>
    public void OnServed()
    {
        if (_orderBubble) _orderBubble.gameObject.SetActive(false);
        if (_guestSpade)  _guestSpade.SetActive(true);
        RequiredCount = 0;
    }

    // ── 내부 ─────────────────────────────────────────────────

    /// <summary>1~5개 가중치 랜덤 (1~3이 더 자주 나옴).</summary>
    private int GetWeightedRandom()
    {
        int r = Random.Range(0, 100);
        if (r < 30) return 1; // 30 %
        if (r < 55) return 2; // 25 %
        if (r < 80) return 3; // 25 %
        if (r < 92) return 4; // 12 %
        return 5;              //  8 %
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
