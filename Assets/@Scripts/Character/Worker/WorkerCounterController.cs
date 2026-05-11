using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static Define;

[Serializable]
public struct CounterWaypoint
{
    public Transform point;
    [Tooltip("도착 후 바라볼 방향 (Y축 월드 오일러각)")]
    public float facingY;
}

[RequireComponent(typeof(NavMeshAgent))]
public class WorkerCounterController : BaseCharacterController
{
    private enum ECounterState
    {
        GoToSupply,
        ReceiveSpade,
        GoToInfoDesk,
        DumpSpade,
        GoToMonitor,
        WatchMonitor,
    }

    [Header("Tray")]
    [SerializeField] private TrayController _tray;
    [SerializeField] private int            _maxTrayCapacity = 10;
    [SerializeField] private float          _receiveInterval = 0.2f;
    [SerializeField] private float          _dumpInterval    = 0.1f;

    [Header("Monitor")]
    [SerializeField] private Vector2 _idleTimeRange = new(3f, 5f);

    private NavMeshAgent        _agent;
    private ECounterState       _state;
    private int                 _lastPileCount;
    private float               _idleTimer;
    private float               _idleThreshold;
    private Coroutine           _rotateCoroutine;

    private CounterWaypoint     _wpSupply;
    private CounterWaypoint     _wpInfoDesk;
    private CounterWaypoint     _wpMonitor;
    private CounterInteraction  _supplyZone;
    private CounterInteraction  _infoDeskZone;
    private SupplyStack         _supplyStack;
    private TrayToItemPlacePile _infoDeskPile;

    protected override void Awake()
    {
        base.Awake();
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed                 = _moveSpeed;
        _agent.angularSpeed          = _rotateSpeed;
        _agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
    }

    private void Start()
    {
        InteractionManager im = InteractionManager.Instance;
        _wpSupply     = im.CounterWpSupply;
        _wpInfoDesk   = im.CounterWpInfoDesk;
        _wpMonitor    = im.CounterWpMonitor;
        _supplyZone   = im.CounterSupplyZone;
        _infoDeskZone = im.CounterInfoDeskZone;
        _supplyStack  = im.CounterSupplyStack;
        _infoDeskPile = im.CounterInfoDeskPile;

        _tray.OnCountChanged += OnTrayCountChanged;

        _supplyZone.InteractInterval  = _receiveInterval;
        _supplyZone.OnInteraction    += HandleSupplyInteraction;

        _infoDeskZone.InteractInterval = _dumpInterval;
        _infoDeskZone.OnInteraction   += HandleInfoDeskInteraction;

        NavigateTo(_wpSupply, ECounterState.GoToSupply);
    }

    private void OnDestroy()
    {
        if (_tray != null)         _tray.OnCountChanged        -= OnTrayCountChanged;
        if (_supplyZone != null)   _supplyZone.OnInteraction   -= HandleSupplyInteraction;
        if (_infoDeskZone != null) _infoDeskZone.OnInteraction -= HandleInfoDeskInteraction;
    }

    private void Update()
    {
        switch (_state)
        {
            case ECounterState.GoToSupply:
                if (IsArrived()) OnArrivedAtSupply();
                break;
            case ECounterState.GoToInfoDesk:
                if (IsArrived()) OnArrivedAtInfoDesk();
                break;
            case ECounterState.GoToMonitor:
                if (IsArrived()) OnArrivedAtMonitor();
                break;
            case ECounterState.WatchMonitor:
                TickWatch();
                break;
        }
    }

    private void NavigateTo(CounterWaypoint wp, ECounterState state)
    {
        if (_rotateCoroutine != null) { StopCoroutine(_rotateCoroutine); _rotateCoroutine = null; }
        SetState(state);
        _agent.isStopped      = false;
        _agent.updateRotation = true;
        _agent.SetDestination(wp.point.position);
    }

    private bool IsArrived()
    {
        if (_agent.pathPending) return false;
        return _agent.remainingDistance <= 0.5f;
    }

    private void FaceDirection(float eulerY)
    {
        _agent.isStopped      = true;
        _agent.updateRotation = false;
        if (_rotateCoroutine != null) StopCoroutine(_rotateCoroutine);
        _rotateCoroutine = StartCoroutine(SmoothRotate(eulerY));
    }

    private IEnumerator SmoothRotate(float targetEulerY)
    {
        Quaternion target = Quaternion.Euler(0f, targetEulerY, 0f);
        while (Quaternion.Angle(transform.rotation, target) > 0.5f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, target, _rotateSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = target;
        _rotateCoroutine   = null;
    }

    private void OnArrivedAtSupply()
    {
        FaceDirection(_wpSupply.facingY);
        SetState(ECounterState.ReceiveSpade);
    }

    private void OnArrivedAtInfoDesk()
    {
        FaceDirection(_wpInfoDesk.facingY);
        SetState(ECounterState.DumpSpade);
    }

    private void OnArrivedAtMonitor()
    {
        FaceDirection(_wpMonitor.facingY);
        _lastPileCount = _infoDeskPile.ObjectCount;
        _idleTimer     = 0f;
        _idleThreshold = UnityEngine.Random.Range(_idleTimeRange.x, _idleTimeRange.y);
        SetState(ECounterState.WatchMonitor);
    }

    private void HandleSupplyInteraction(WorkerCounterController _)
    {
        if (_state != ECounterState.ReceiveSpade) return;
        if (_tray.TotalCount >= _maxTrayCapacity) return;

        GameObject item = _supplyStack.RemoveFromPile();
        if (item == null) return;

        _tray.ReceiveItem(item);

        if (_tray.TotalCount >= _maxTrayCapacity)
            NavigateTo(_wpInfoDesk, ECounterState.GoToInfoDesk);
    }

    private void HandleInfoDeskInteraction(WorkerCounterController _)
    {
        if (_state != ECounterState.DumpSpade) return;

        GameObject item = _tray.TakeItem();
        if (item != null) _infoDeskPile.ReceiveItem(item);

        if (!_tray.HasItems)
            NavigateTo(_wpMonitor, ECounterState.GoToMonitor);
    }

    private void TickWatch()
    {
        int current = _infoDeskPile.ObjectCount;

        if (current != _lastPileCount)
        {
            _lastPileCount = current;
            _idleTimer     = 0f;
        }
        else
        {
            _idleTimer += Time.deltaTime;
            if (_idleTimer >= _idleThreshold)
                NavigateTo(_wpSupply, ECounterState.GoToSupply);
        }
    }

    private void OnTrayCountChanged(int _) => UpdateAnimation();

    private void SetState(ECounterState newState)
    {
        _state = newState;
        UpdateAnimation();
    }

    protected override void UpdateAnimation()
    {
        bool moving   = _state is ECounterState.GoToSupply or ECounterState.GoToInfoDesk or ECounterState.GoToMonitor;
        bool hasItems = _tray != null && _tray.HasItems;

        int hash = (moving, hasItems) switch
        {
            (true,  true)  => SERVING_MOVE,
            (true,  false) => MOVE,
            (false, true)  => SERVING_IDLE,
            _              => IDLE,
        };

        _animator.CrossFade(hash, 0.1f);
    }
}
