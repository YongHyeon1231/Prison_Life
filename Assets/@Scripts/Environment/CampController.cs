using TMPro;
using UnityEngine;
using static Define;

/// <summary>
/// @Camp 루트에 붙이는 컨트롤러.
///
/// Inspector 연결
///   _animator          : @Camp의 Animator
///   _doorInteraction   : 문 트리거의 GuestInteraction
///   _campSpots         : 캠프 내부 자리 Waypoints
///   _campIdleFacingY   : 캠프 내 Idle 시 바라볼 방향 Y 회전값 (문 쪽 각도 입력)
///   _countText         : 현재 인원 / 최대 인원 표시 TMP
///   _maxCapacity       : 최대 수용 인원 (캠프 레벨 업 시 SetMaxCapacity 호출)
/// </summary>
public class CampController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator         _animator;
    [SerializeField] private GuestInteraction _doorInteraction;
    [SerializeField] private Waypoints        _campSpots;
    [SerializeField] private float             _campIdleFacingY;
    [SerializeField] private TextMeshProUGUI  _countText;

    [Header("Settings")]
    [SerializeField] private int _maxCapacity = 11;

    private GuestController[] _spotOccupants;
    private GuestController   _lastDoorGuest;

    private int GuestCount
    {
        get
        {
            int count = 0;
            foreach (var g in _spotOccupants)
                if (g != null) count++;
            return count;
        }
    }

    // ── 초기화 ────────────────────────────────────────────────────

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _spotOccupants = new GuestController[_campSpots.GetPointCount()];
        _doorInteraction.OnGuestEntered += OnGuestAtDoor;
        UpdateText();
    }

    private void OnDestroy()
    {
        if (_doorInteraction != null)
            _doorInteraction.OnGuestEntered -= OnGuestAtDoor;
    }

    // ── 문 ───────────────────────────────────────────────────────

    private void OnGuestAtDoor(GuestController guest)
    {
        _lastDoorGuest = guest;
        _animator.Play(CAMP_DOOR_OPEN);
    }

    // ── 바닥 (CampFloorZone이 호출) ───────────────────────────────

    public void OnGuestEnteredFloor(GuestController guest)
    {
        if (guest == _lastDoorGuest)
        {
            _animator.Play(CAMP_DOOR_CLOSE);
            _lastDoorGuest = null;
        }

        AssignSpot(guest);
        UpdateText();
    }

    public void OnGuestExitedFloor(GuestController guest)
    {
        FreeSpot(guest);
        AdvanceSpots();
        UpdateText();
    }

    // ── 레벨업 ────────────────────────────────────────────────────

    public void SetMaxCapacity(int capacity)
    {
        _maxCapacity = capacity;
        UpdateText();
    }

    // ── 자리 관리 ─────────────────────────────────────────────────

    private void AssignSpot(GuestController guest)
    {
        int slot = FindEmptySlot();
        if (slot < 0) return;

        _spotOccupants[slot] = guest;
        MoveGuestToSlot(guest, slot);
    }

    private void FreeSpot(GuestController guest)
    {
        for (int i = 0; i < _spotOccupants.Length; i++)
        {
            if (_spotOccupants[i] != guest) continue;
            _spotOccupants[i] = null;
            return;
        }
    }

    /// <summary>빈 슬롯이 생기면 뒤 게스트를 앞으로 당겨옵니다.</summary>
    private void AdvanceSpots()
    {
        for (int i = 0; i < _spotOccupants.Length; i++)
        {
            if (_spotOccupants[i] != null) continue;

            for (int j = i + 1; j < _spotOccupants.Length; j++)
            {
                if (_spotOccupants[j] == null) continue;

                _spotOccupants[i] = _spotOccupants[j];
                _spotOccupants[j] = null;
                MoveGuestToSlot(_spotOccupants[i], i);
                break;
            }
        }
    }

    private void MoveGuestToSlot(GuestController guest, int slot)
    {
        Quaternion facing = Quaternion.Euler(0f, _campIdleFacingY, 0f);

        guest.WalkToWaitPoint(_campSpots.GetPoint(slot).position, facing);
    }

    private int FindEmptySlot()
    {
        for (int i = 0; i < _spotOccupants.Length; i++)
            if (_spotOccupants[i] == null) return i;
        return -1;
    }

    // ── UI ───────────────────────────────────────────────────────

    private void UpdateText()
    {
        if (_countText != null)
            _countText.text = $"{GuestCount} / {_maxCapacity}";
    }
}
