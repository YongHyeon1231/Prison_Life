using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static Define;

/// <summary>
/// Mining Worker AI.
/// NavMeshAgent로 _patrolA ↔ _patrolB 왕복하며 Rock을 감지하면 채굴 후
/// PutDownTheRockPile에 직접 전달합니다.
///
/// 프리팹 설정:
///   - CapsuleCollider  (Is Trigger = true)  : Rock 감지, Player 통과 허용
///   - Rigidbody        (Is Kinematic = true) : Trigger↔Trigger 감지에 필요
///   - NavMeshAgent                           : 이동 및 경로 탐색
/// 씬에 MiningArea 컴포넌트가 있어야 패트롤 경로와 Pile 정보를 받습니다.
/// </summary>
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
        if (area == null)
        {
            Debug.LogWarning("[MiningWorkerController] InteractionManager에 MiningArea가 설정되지 않았습니다.");
            return;
        }

        if (!area.TryGetNextLane(out _patrolA, out _patrolB))
        {
            Debug.LogWarning("[MiningWorkerController] MiningArea에 남은 패트롤 줄이 없습니다. Lanes 배열을 확인하세요.");
            return;
        }

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

        // Rock이 채굴되거나 사라질 때까지 Mine 반복
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
            // 1타 완료, Rock 아직 존재 → Mine 상태 유지하며 루프 반복
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
