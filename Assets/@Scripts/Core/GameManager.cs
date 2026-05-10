using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("Item Prefabs")]
    [SerializeField] private GameObject _rockPrefab;
    [SerializeField] private GameObject _spadePrefab;

    public Vector2 JoystickDir { get; set; } = Vector2.zero;

    public ResourceManager Resource  { get; } = new ResourceManager();
    public InventoryManager Inventory { get; } = new InventoryManager();

    protected override void Awake()
    {
        base.Awake();

        if (_rockPrefab  != null) Inventory.RegisterPrefab(InventoryItemType.Rock,  _rockPrefab);
        if (_spadePrefab != null) Inventory.RegisterPrefab(InventoryItemType.Spade, _spadePrefab);
    }
}
