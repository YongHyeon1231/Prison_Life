using UnityEngine;

/// <summary>
/// 씬에 배치된 Interaction 관련 오브젝트 레퍼런스를 보관하는 매니저.
/// 프리팹·캐릭터·씬 스크립트가 씬 오브젝트를 직접 탐색(GetComponentInParent, FindChild,
/// FindObjectOfType 등)하지 않고 이곳에서 가져갑니다.
/// </summary>
public class InteractionManager : Singleton<InteractionManager>
{
    // ── Player ────────────────────────────────────────────────
    [Header("Player - Supply Stack")]
    [SerializeField] private PlayerInteraction _supplyPlayerZone;

    [Header("Player - InfoDesk Input")]
    [SerializeField] private PlayerInteraction _infoDeskPlayerZone;

    // ── Guest ─────────────────────────────────────────────────
    [Header("Guest - Camp Door")]
    [SerializeField] private GuestInteraction _campDoorGuestZone;

    // ── Mining Worker ─────────────────────────────────────────
    [Header("Mining Worker")]
    [SerializeField] private MiningArea _miningArea;

    // ── Counter Worker ────────────────────────────────────────
    [Header("Counter Worker - Waypoints")]
    [SerializeField] private CounterWaypoint _counterWpSupply;
    [SerializeField] private CounterWaypoint _counterWpInfoDesk;
    [SerializeField] private CounterWaypoint _counterWpMonitor;

    [Header("Counter Worker - Zones")]
    [SerializeField] private CounterInteraction _counterSupplyZone;
    [SerializeField] private CounterInteraction _counterInfoDeskZone;

    [Header("Counter Worker - References")]
    [SerializeField] private SupplyStack         _counterSupplyStack;
    [SerializeField] private TrayToItemPlacePile _counterInfoDeskPile;

    // ── Properties ────────────────────────────────────────────
    public PlayerInteraction   SupplyPlayerZone    => _supplyPlayerZone;
    public PlayerInteraction   InfoDeskPlayerZone  => _infoDeskPlayerZone;
    public GuestInteraction    CampDoorGuestZone   => _campDoorGuestZone;
    public MiningArea          MiningArea          => _miningArea;
    public CounterWaypoint     CounterWpSupply     => _counterWpSupply;
    public CounterWaypoint     CounterWpInfoDesk   => _counterWpInfoDesk;
    public CounterWaypoint     CounterWpMonitor    => _counterWpMonitor;
    public CounterInteraction  CounterSupplyZone   => _counterSupplyZone;
    public CounterInteraction  CounterInfoDeskZone => _counterInfoDeskZone;
    public SupplyStack         CounterSupplyStack  => _counterSupplyStack;
    public TrayToItemPlacePile CounterInfoDeskPile => _counterInfoDeskPile;
}
