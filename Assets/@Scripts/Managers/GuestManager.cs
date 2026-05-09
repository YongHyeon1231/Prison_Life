using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 게스트 스폰 · 대기열 · 서빙 · 별 보상을 통합 관리합니다.
///
/// Inspector 연결
///   _guestPrefab   : GuestController 프리팹
///   _spawnPoint    : 게스트 스폰 위치 Transform
///   _waitPoints    : 대기 위치를 가진 Waypoints (인덱스 0이 맨 앞)
///   _itemPlace     : Counter의 TrayToItemPlacePile
///   _starPile      : GetStar 오브젝트의 AnimatedPile
///   _exitDistance  : 서빙 완료 후 게스트가 앞으로 걸어갈 거리
/// </summary>
public class GuestManager : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GuestController _guestPrefab;

    [Header("References")]
    [SerializeField] private Transform           _spawnPoint;
    [SerializeField] private Transform           _guestContainer;
    [SerializeField] private Transform           _guestWaitPoint;
    [SerializeField] private Waypoints           _waitPoints;
    [SerializeField] private TrayToItemPlacePile _itemPlace;
    [SerializeField] private AnimatedPile        _starPile;

    [Header("Settings")]
    [SerializeField] private float _spawnInterval    = 3f;
    [SerializeField] private float _serveJumpPower   = 6f;
    [SerializeField] private float _serveJumpDuration = 0.3f;

    private readonly List<GuestController> _queue = new();
    private bool _isServing = false;

    // ── 초기화 ────────────────────────────────────────────────

    private void Start()
    {
        if (_guestPrefab == null)
        {
            Debug.LogError("[GuestManager] _guestPrefab이 Inspector에 연결되지 않았습니다.");
            return;
        }
        if (_spawnPoint == null)
        {
            Debug.LogError("[GuestManager] _spawnPoint가 Inspector에 연결되지 않았습니다.");
            return;
        }
        if (_waitPoints == null)
        {
            Debug.LogError("[GuestManager] _waitPoints가 Inspector에 연결되지 않았습니다.");
            return;
        }

        _itemPlace.OnCountChanged += _ => TryServe();
        StartCoroutine(CoSpawnGuest());
    }

    // ── Update : 큐 AI ────────────────────────────────────────

    private void Update()
    {
        AdvanceQueue();
        CheckFrontArrival();
    }

    // ── 스폰 ──────────────────────────────────────────────────

    private IEnumerator CoSpawnGuest()
    {
        while (true)
        {
            yield return new WaitForSeconds(_spawnInterval);

            if (_queue.Count >= _waitPoints.GetPointCount())
                continue;

            Vector3 spawnPos = _spawnPoint.position;

            if (!NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            {
                Debug.LogWarning("[GuestManager] 스폰 위치 근처에 NavMesh가 없습니다.");
                continue;
            }
            spawnPos = hit.position;

            GuestController guest = Instantiate(_guestPrefab, spawnPos, _spawnPoint.rotation, _guestContainer);
            guest.gameObject.SetActive(true);

            yield return null; // NavMeshAgent가 NavMesh에 스냅될 때까지 한 프레임 대기

            int backIndex = _waitPoints.GetPointCount() - 1;
            guest.CurrentQueueIndex = backIndex;
            guest.SetDestination(_waitPoints.GetPoint(backIndex).position);
            _queue.Add(guest);
        }
    }

    // ── 큐 이동 ───────────────────────────────────────────────

    /// <summary>도착한 게스트를 가능한 한 앞 칸으로 전진시킵니다.</summary>
    private void AdvanceQueue()
    {
        for (int i = 0; i < _queue.Count; i++)
        {
            GuestController guest = _queue[i];
            if (!guest.HasArrived) continue;
            if (guest.CurrentQueueIndex <= i) continue;

            guest.CurrentQueueIndex = i;
            guest.SetDestination(_waitPoints.GetPoint(i).position);
        }
    }

    /// <summary>맨 앞 게스트가 index 0에 도착하면 주문을 표시합니다.</summary>
    private void CheckFrontArrival()
    {
        if (_queue.Count == 0) return;

        GuestController front = _queue[0];
        if (!front.HasArrived) return;
        if (front.CurrentQueueIndex != 0) return;
        if (front.RequiredCount > 0) return; // 이미 주문 표시 중

        front.ShowOrder();
        TryServe();
    }

    // ── 서빙 ──────────────────────────────────────────────────

    private void TryServe()
    {
        if (_isServing) return;
        if (_queue.Count == 0) return;

        GuestController front = _queue[0];
        if (front.RequiredCount == 0) return;
        if (!front.HasArrived) return;
        if (_itemPlace.ObjectCount < front.RequiredCount) return;

        _isServing = true;
        StartCoroutine(CoServe(front));
    }

    private IEnumerator CoServe(GuestController guest)
    {
        int count     = guest.RequiredCount;
        int remaining = count;

        for (int i = 0; i < count; i++)
        {
            GameObject item = _itemPlace.RemoveFromPile();
            if (item == null) { remaining--; continue; }

            Vector3 dest = guest.transform.position + Vector3.up * 1.2f;
            item.transform.DOJump(dest, _serveJumpPower, 1, _serveJumpDuration)
                .OnComplete(() =>
                {
                    Destroy(item);
                    remaining--;
                });

            yield return new WaitForSeconds(0.08f); // 아이템 순차 이동
        }

        yield return new WaitUntil(() => remaining <= 0);

        // 게스트 서빙 완료
        guest.OnServed();

        // 별 보상 : 받은 수량 × 2
        for (int i = 0; i < count * 2; i++)
            _starPile.AddItem();

        // 큐에서 제거 후 WaitPoint로 이동
        _queue.RemoveAt(0);

        guest.WalkToWaitPoint(_guestWaitPoint.position, _guestWaitPoint.rotation);

        _isServing = false;
        TryServe();
    }
}
