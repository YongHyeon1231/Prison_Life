using System;

/// <summary>
/// 게임 내 자원 수량을 중앙 관리합니다.
/// GameManager.Instance.Resource 를 통해 접근합니다.
/// </summary>
public class ResourceManager
{
    // ── Rock ──────────────────────────────────────────────────

    public int RockCount { get; private set; }
    public event Action<int> OnRockCountChanged;

    public void SetRockCount(int count)
    {
        RockCount = count;
        OnRockCountChanged?.Invoke(count);
    }

    // ── Spade ─────────────────────────────────────────────────

    public int SpadeCount { get; private set; }
    public event Action<int> OnSpadeCountChanged;

    public void SetSpadeCount(int count)
    {
        SpadeCount = count;
        OnSpadeCountChanged?.Invoke(count);
    }
}
