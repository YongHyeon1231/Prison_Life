using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockPool : Singleton<RockPool>
{
    [Header("채굴 후 리스폰 딜레이 (초)")]
    [SerializeField] private float _respawnDelay = 5f;

    private List<Rock> _allRocks = new List<Rock>();

    public IReadOnlyList<Rock> AllRocks => _allRocks;

    protected override void Awake()
    {
        base.Awake();

        foreach (var rock in FindObjectsByType<Rock>(FindObjectsSortMode.None))
        {
            rock.OriginPosition = rock.transform.position;
            _allRocks.Add(rock);
        }
    }

    public void ReturnRock(Rock rock)
    {
        StartCoroutine(RespawnAfterDelay(rock, _respawnDelay));
    }

    private IEnumerator RespawnAfterDelay(Rock rock, float delay)
    {
        yield return new WaitForSeconds(delay);
        rock.Respawn();
    }

    public List<Rock> GetRocksAtPositions(List<Vector3> worldPositions, float snapRadius = 0.8f)
    {
        var result = new List<Rock>();
        foreach (var pos in worldPositions)
        {
            float minDist = float.MaxValue;
            Rock  nearest = null;

            foreach (var rock in _allRocks)
            {
                if (!rock.IsAvailable) continue;
                float d = Vector3.Distance(rock.transform.position, pos);
                if (d < snapRadius && d < minDist)
                {
                    minDist = d;
                    nearest = rock;
                }
            }

            if (nearest != null && !result.Contains(nearest))
                result.Add(nearest);
        }
        return result;
    }

    public List<Rock> GetAvailableRocksInRadius(Vector3 center, float radius)
    {
        var result = new List<Rock>();
        float sqr = radius * radius;
        foreach (var rock in _allRocks)
        {
            if (!rock.IsAvailable) continue;
            if ((rock.transform.position - center).sqrMagnitude <= sqr)
                result.Add(rock);
        }
        return result;
    }
}
