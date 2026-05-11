using UnityEngine;

public class InteractionManager : Singleton<InteractionManager>
{
    [Header("Player - Supply Stack")]
    [SerializeField] private PlayerInteraction _supplyPlayerZone;

    [Header("Player - InfoDesk Input")]
    [SerializeField] private PlayerInteraction _infoDeskPlayerZone;

    [Header("Guest - Camp Door")]
    [SerializeField] private GuestInteraction _campDoorGuestZone;

    [Header("Mining Worker")]
    [SerializeField] private MiningArea _miningArea;

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
