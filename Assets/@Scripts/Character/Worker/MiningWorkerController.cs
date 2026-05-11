using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static Define;

[RequireComponent(typeof(NavMeshAgent))]
public class MiningWorkerController : BaseCharacterController
{
    [Header("Mining")]
    [SerializeField] private float _miningDuration = 1.5f;

    private NavMeshAgent       _agent;
    private EState             _state = EState.Idle;
    private bool               _movingToB = true;
    private Transform          _patrolA;
    private Transform          _patrolB;
    private PutDownTheRockPile _targetPile;
    private Rock               _currentRock;

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
        MiningArea area = InteractionManager.Instance.MiningArea;
        if (area == null) return;

        if (!area.TryGetNextLane(out _patrolA, out _patrolB)) return;

        _targetPile = area.TargetPile;

        if (_patrolA == null || _patrolB == null) return;

        SetState(EState.Move);
        _agent.SetDestination(_patrolB.position);
    }

    private void Update()
    {
        if (_state == EState.Move)
            CheckPatrolEnd();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_state != EState.Move) return;
        if (!other.CompareTag("Rock")) return;

        Rock rock = other.GetComponent<Rock>();
        if (rock == null || !rock.IsAvailable) return;

        _currentRock = rock;
        StartCoroutine(MineCoroutine());
    }

    private IEnumerator MineCoroutine()
    {
        SetState(EState.Mine);
        _agent.isStopped      = true;
        _agent.updateRotation = false;

        Vector3 dir = _currentRock.transform.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(dir);

        while (_currentRock != null && _currentRock.IsAvailable)
        {
            yield return new WaitForSeconds(_miningDuration);

            if (_currentRock == null || !_currentRock.IsAvailable)
                break;

            if (_currentRock.TryHit())
            {
                DeliverRock(_currentRock);
                _currentRock.Mine();
                break;
            }
        }

        _currentRock          = null;
        _agent.isStopped      = false;
        _agent.updateRotation = true;
        SetState(EState.Move);
    }

    private void DeliverRock(Rock rock)
    {
        if (_targetPile == null) return;

        GameObject prefab = GameManager.Instance.Inventory.GetPrefab(InventoryItemType.Rock);
        if (prefab == null) return;

        GameObject item = Instantiate(prefab, rock.transform.position, Quaternion.identity);
        _targetPile.ReceiveItem(item);
    }

    private void CheckPatrolEnd()
    {
        if (_agent.pathPending) return;
        if (_agent.remainingDistance > 0.5f) return;

        _movingToB = !_movingToB;
        _agent.SetDestination(_movingToB ? _patrolB.position : _patrolA.position);
    }

    private void SetState(EState newState)
    {
        _state = newState;
        UpdateAnimation();
    }

    protected override void UpdateAnimation()
    {
        switch (_state)
        {
            case EState.Idle: _animator.CrossFade(IDLE, 0.1f);  break;
            case EState.Move: _animator.CrossFade(MOVE, 0.05f); break;
            case EState.Mine: _animator.CrossFade(MINE, 0.1f);  break;
        }
    }
}
