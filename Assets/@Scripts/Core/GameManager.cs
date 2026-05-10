using System.Collections.Generic;
using UnityEngine;
using static Define;

public class GameManager : Singleton<GameManager>
{
    [Header("Item Prefabs")]
    [SerializeField] private GameObject _rockPrefab;
    [SerializeField] private GameObject _spadePrefab;

    [Header("Player")]
    [SerializeField] private PlayerController _player;

    [Header("Worker Prefabs")]
    [SerializeField] private GameObject _miningWorkerPrefab;
    [SerializeField] private GameObject _counterWorkerPrefab;

    public Vector2          JoystickDir { get; set; } = Vector2.zero;
    public PlayerController Player      => _player;

    public ResourceManager Resource  { get; } = new ResourceManager();
    public InventoryManager Inventory { get; } = new InventoryManager();

    private Dictionary<WorkerType, GameObject> _workerPrefabs;

    protected override void Awake()
    {
        base.Awake();

        if (_rockPrefab  != null) Inventory.RegisterPrefab(InventoryItemType.Rock,  _rockPrefab);
        if (_spadePrefab != null) Inventory.RegisterPrefab(InventoryItemType.Spade, _spadePrefab);

        _workerPrefabs = new Dictionary<WorkerType, GameObject>
        {
            { WorkerType.Mining,  _miningWorkerPrefab  },
            { WorkerType.Counter, _counterWorkerPrefab },
        };
    }

    public GameObject GetWorkerPrefab(WorkerType type)
    {
        if (_workerPrefabs.TryGetValue(type, out GameObject prefab) && prefab != null)
            return prefab;

        Debug.LogWarning($"[GameManager] {type} Worker 프리팹이 등록되지 않았습니다.");
        return null;
    }
}
