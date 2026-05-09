using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어가 현재 소지한 아이템 수량 및 프리팹을 관리합니다.
/// GameManager.Instance.Inventory 를 통해서만 접근합니다.
/// </summary>
public enum InventoryItemType { Rock, Spade, Star }

public class InventoryManager
{
    private readonly Dictionary<InventoryItemType, int>        _counts  = new();
    private readonly Dictionary<InventoryItemType, GameObject> _prefabs = new();

    /// <summary>아이템 종류와 현재 수량을 전달합니다.</summary>
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
}
