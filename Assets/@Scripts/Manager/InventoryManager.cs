using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class InventoryManager
{
    private readonly Dictionary<InventoryItemType, int>        _counts  = new();
    private readonly Dictionary<InventoryItemType, GameObject> _prefabs = new();

    public event Action<InventoryItemType, int> OnCountChanged;

    public int GetCount(InventoryItemType type) =>
        _counts.TryGetValue(type, out int v) ? v : 0;

    public void SetCount(InventoryItemType type, int count)
    {
        _counts[type] = count;
        OnCountChanged?.Invoke(type, count);
    }

    public void Add(InventoryItemType type, int amount)
    {
        int next = GetCount(type) + amount;
        SetCount(type, next);
    }

    public bool Spend(InventoryItemType type, int amount)
    {
        int current = GetCount(type);
        if (current < amount) return false;
        SetCount(type, current - amount);
        return true;
    }

    public void RegisterPrefab(InventoryItemType type, GameObject prefab) =>
        _prefabs[type] = prefab;

    public GameObject GetPrefab(InventoryItemType type) =>
        _prefabs.TryGetValue(type, out var p) ? p : null;

    private static InventoryItemType MapResource(ResourceType type) => type switch
    {
        ResourceType.Star => InventoryItemType.Star,
        ResourceType.Rock => InventoryItemType.Rock,
        _                 => throw new NotImplementedException($"ResourceType.{type} is not mapped to InventoryItemType")
    };

    public int  GetResourceCount(ResourceType type)          => GetCount(MapResource(type));
    public bool SpendResource   (ResourceType type, int amt) => Spend(MapResource(type), amt);
    public void AddResource     (ResourceType type, int amt) => Add(MapResource(type), amt);
}
