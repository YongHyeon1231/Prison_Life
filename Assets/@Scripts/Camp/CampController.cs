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

    [Header("Upgrade Tier Effects")]
    [SerializeField] private List<CampUpgradeTierEffect> _upgradeTiers;

    [Header("Settings")]
    [SerializeField] private int _maxCapacity = 12;

    private GuestInteraction  _doorInteraction;
    private GuestController[] _spotOccupants;
    private GuestController[] _overflowOccupants;
    private GuestController   _lastDoorGuest;
    private bool              _fullCutsceneFired;
    private int               _upgradeIndex;
    private Transform         _pendingWall;
    private Vector3           _pendingWallPos;
    private readonly List<GameObject> _persistentActivations = new List<GameObject>();

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

    private void LateUpdate()
    {
        if (_pendingWall != null)
            _pendingWall.localPosition = _pendingWallPos;

        foreach (var obj in _persistentActivations)
            if (obj != null && !obj.activeInHierarchy)
                ActivateWithParents(obj);
    }

    private static void ActivateWithParents(GameObject obj)
    {
        Transform t = obj.transform.parent;
        while (t != null)
        {
            if (!t.gameObject.activeSelf) t.gameObject.SetActive(true);
            t = t.parent;
        }
        obj.SetActive(true);
    }

    private void OnDestroy()
    {
        if (_doorInteraction != null)
            _doorInteraction.OnEntered -= OnGuestAtDoor;
    }

    private void OnGuestAtDoor(GuestController guest)
    {
        _lastDoorGuest = guest;
        _animator.Play(CAMP_DOOR_OPEN);
    }

    public void OnGuestEnteredFloor(GuestController guest)
    {
        if (guest == _lastDoorGuest)
        {
            _animator.Play(CAMP_DOOR_CLOSE);
            _lastDoorGuest = null;
        }

        if (IsAtCapacity)
        {
            TryRouteToOverflow(guest);
            return;
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

    public void Upgrade()
    {
        int[] tiers = { 12, 24 };
        for (int i = 0; i < tiers.Length - 1; i++)
        {
            if (_maxCapacity != tiers[i]) continue;
            _maxCapacity = tiers[i + 1];
            break;
        }

        _fullCutsceneFired = false;
        UpdateText();

        if (_cutsceneController != null)
            _cutsceneController.PlayCampUpgrade(OnUpgradeCutsceneComplete);
        else
            OnUpgradeCutsceneComplete();
    }

    private void OnUpgradeCutsceneComplete()
    {
        ApplyUpgradeTierEffect();

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
        {
            foreach (var obj in effect.objectsToActivate)
            {
                if (obj == null) continue;
                ActivateWithParents(obj);
                _persistentActivations.Add(obj);
            }
        }

        if (effect.wallToMove != null)
        {
            if (effect.wallToMove.TryGetComponent<Animator>(out var wallAnim))
                wallAnim.enabled = false;

            _pendingWall    = effect.wallToMove;
            _pendingWallPos = effect.wallTargetPos;
        }

        if (effect.nextCampSpots != null)
        {
            _campSpots = effect.nextCampSpots;
            var old = _spotOccupants;
            _spotOccupants = new GuestController[_campSpots.GetPointCount()];
            for (int i = 0; i < old.Length && i < _spotOccupants.Length; i++)
                _spotOccupants[i] = old[i];
        }

        _upgradeIndex++;
    }

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

    private void UpdateText()
    {
        if (_countText != null)
            _countText.text = $"{GuestCount} / {_maxCapacity}";
    }
}
