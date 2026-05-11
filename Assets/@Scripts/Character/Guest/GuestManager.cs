using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

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

    [Header("Camp")]
    [SerializeField] private CampController _campController;

    [Header("Settings")]
    [SerializeField] private float _spawnInterval     = 3f;
    [SerializeField] private float _serveJumpPower    = 6f;
    [SerializeField] private float _serveJumpDuration = 0.3f;

    private readonly List<GuestController> _queue = new();
    private bool _isServing = false;

    private void Start()
    {
        if (_guestPrefab == null || _spawnPoint == null || _waitPoints == null) return;

        _itemPlace.OnCountChanged += _ => TryServe();
        StartCoroutine(CoSpawnGuest());
    }

    private void Update()
    {
        AdvanceQueue();
        CheckFrontArrival();
    }

    private IEnumerator CoSpawnGuest()
    {
        while (true)
        {
            yield return new WaitForSeconds(_spawnInterval);

            if (_queue.Count >= _waitPoints.GetPointCount())
                continue;

            Vector3 spawnPos = _spawnPoint.position;

            if (!NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, 3f, NavMesh.AllAreas))
                continue;

            spawnPos = hit.position;

            GuestController guest = Instantiate(_guestPrefab, spawnPos, _spawnPoint.rotation, _guestContainer);
            guest.gameObject.SetActive(true);

            yield return null;

            int backIndex = _waitPoints.GetPointCount() - 1;
            guest.CurrentQueueIndex = backIndex;
            guest.SetDestination(_waitPoints.GetPoint(backIndex).position);
            _queue.Add(guest);
        }
    }

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

    private void CheckFrontArrival()
    {
        if (_queue.Count == 0) return;

        GuestController front = _queue[0];
        if (!front.HasArrived) return;
        if (front.CurrentQueueIndex != 0) return;
        if (front.RequiredCount > 0) return;

        front.ShowOrder();
        TryServe();
    }

    private void TryServe()
    {
        if (_isServing) return;
        if (_queue.Count == 0) return;

        GuestController front = _queue[0];
        if (front.RequiredCount == 0) return;
        if (!front.HasArrived) return;
        if (_itemPlace.ObjectCount < front.RequiredCount) return;
        if (_campController != null && _campController.IsAtCapacity && _campController.IsOverflowFull) return;

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

            yield return new WaitForSeconds(0.08f);
        }

        yield return new WaitUntil(() => remaining <= 0);

        guest.OnServed();

        for (int i = 0; i < count * 7; i++)
            _starPile.AddItem();

        _queue.RemoveAt(0);

        if (_campController != null && _campController.IsAtCapacity)
            _campController.TryRouteToOverflow(guest);
        else
            guest.WalkToWaitPoint(_guestWaitPoint.position, _guestWaitPoint.rotation);

        _isServing = false;
        TryServe();
    }
}
