using System;

public class ResourceManager
{
    public int RockCount { get; private set; }
    public event Action<int> OnRockCountChanged;

    public void SetRockCount(int count)
    {
        RockCount = count;
        OnRockCountChanged?.Invoke(count);
    }

    public int SpadeCount { get; private set; }
    public event Action<int> OnSpadeCountChanged;

    public void SetSpadeCount(int count)
    {
        SpadeCount = count;
        OnSpadeCountChanged?.Invoke(count);
    }
}
