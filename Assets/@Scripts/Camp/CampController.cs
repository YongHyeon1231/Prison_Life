using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Define;

[Serializable]
public class CampUpgradeTierEffect
{
    public List<GameObject> objectsToActivate;
    public Transform        wallToMove;
    public Vector3          wallTargetPos;
    public Waypoints        nextCampSpots;
}

public class CampController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator            _animator;
    [SerializeField] private Waypoints           _campSpots;
    [SerializeField] private float               _campIdleFacingY;
    [SerializeField] private TextMeshProUGUI     _countText;
    [SerializeField] private CutsceneController  _cutsceneController;

    [Header("Overflow")]
    [SerializeField] private Waypoints _overflowSpots;

    [Header("Upgrade Tier Effects (index 0 = 10→20, 1 = 20→30)")]
    [SerializeField] private List<CampUpgradeTierEffect> _upgradeTiers;

    [Header("Settings")]
    [SerializeField] private int _maxCapacity = 10;

    private GuestInteraction  _doorInteraction;
    private GuestController[] _spotOccupants;
    private GuestController[] _overflowOccupants;
    private GuestController   _lastDoorGuest;
    private bool              _fullCutsceneFired;
    private int               _upgradeIndex;

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

    public int  MaxCapacity  => _maxCapacity;
    public bool IsAtCapacity => GuestCount >= _maxCapacity;
    public bool IsOverflowFull
    {
        get
        {
            if (_overflowOccupants == null || _overflowOccupants.Length == 0) return true;
            foreach (var g in _overflowOccupants)
                if (g == null) return false;
            return true;
        }
    }

    // ── 초기화 ────────────────────────────────────────────────────

    private void Start()
    {
        _animator          = GetComponent<Animator>();
        _spotOccupants     = new GuestController[_campSpots.GetPointCount()];
        int overflowCount  = _overflowSpots != null ? _overflowSpots.GetPointCount() : 0;
        _overflowOccupants = new GuestController[overflowCount];
        _doorInteraction   = InteractionManager.Instance.CampDoorGuestZone;
        _doorInteraction.OnEntered += OnGuestAtDoor;
        UpdateText();
    }

    private void OnDestroy()
    {
        if (_doorInteraction != null)
            _doorInteraction.OnEntered -= OnGuestAtDoor;
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

        if (!_fullCutsceneFired && GuestCount >= _maxCapacity)
        {
            _fullCutsceneFired = true;
            if (_cutsceneController != null)
                _cutsceneController.PlayCampFull();
        }
    }

    public void OnGuestExitedFloor(GuestController guest)
    {
        FreeSpot(guest);
        AdvanceSpots();
        UpdateText();
    }

    // ── 오버플로우 ────────────────────────────────────────────────

    public bool TryRouteToOverflow(GuestController guest)
    {
        for (int i = 0; i < _overflowOccupants.Length; i++)
        {
            if (_overflowOccupants[i] != null) continue;
            _overflowOccupants[i] = guest;
            guest.ShowNoSpace();
            Quaternion facing = Quaternion.Euler(0f, _campIdleFacingY, 0f);
            guest.WalkToWaitPoint(_overflowSpots.GetPoint(i).position, facing);
            return true;
        }
        return false;
    }

    // ── 업그레이드 ────────────────────────────────────────────────

    public void Upgrade()
    {
        int[] tiers = { 10, 20, 30 };
        for (int i = 0; i < tiers.Length - 1; i++)
        {
            if (_maxCapacity != tiers[i]) continue;
            _maxCapacity = tiers[i + 1];
            break;
        }

        ApplyUpgradeTierEffect();

        _fullCutsceneFired = false;
        UpdateText();

        for (int i = 0; i < _overflowOccupants.Length; i++)
        {
            if (_overflowOccupants[i] == null) continue;
            GuestController g = _overflowOccupants[i];
            _overflowOccupants[i] = null;
            g.HideNoSpace();
            g.SetDestination(_campSpots.GetPoint(0).position);
        }
    }

    private void ApplyUpgradeTierEffect()
    {
        if (_upgradeTiers == null || _upgradeIndex >= _upgradeTiers.Count)
        {
            _upgradeIndex++;
            return;
        }

        CampUpgradeTierEffect effect = _upgradeTiers[_upgradeIndex];

        if (effect.objectsToActivate != null)
            foreach (var obj in effect.objectsToActivate)
                if (obj != null) obj.SetActive(true);

        if (effect.wallToMove != null)
            effect.wallToMove.localPosition = effect.wallTargetPos;

        if (effect.nextCampSpots != null)
        {
            _campSpots = effect.nextCampSpots;
            // 기존 occupants 유지하면서 배열 확장
            var old = _spotOccupants;
            _spotOccupants = new GuestController[_campSpots.GetPointCount()];
            for (int i = 0; i < old.Length && i < _spotOccupants.Length; i++)
                _spotOccupants[i] = old[i];
        }

        _upgradeIndex++;
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
        int limit = Mathf.Min(_maxCapacity, _spotOccupants.Length);
        for (int i = 0; i < limit; i++)
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
