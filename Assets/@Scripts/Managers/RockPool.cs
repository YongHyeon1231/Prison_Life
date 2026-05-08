using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rock 풀 관리자.
/// Rock 을 Destroy 하지 않고 SetActive(false) → 일정 시간 뒤 Respawn() 으로 재사용합니다.
/// 씬에 배치된 모든 Rock 을 Awake 시 자동 수집합니다.
/// </summary>
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

    // ──────────────────────────────────────────────
    //  풀 반환 (Rock.Mine() 에서 호출)
    // ──────────────────────────────────────────────

    /// <summary>
    /// 채굴된 Rock 을 받아 리스폰 타이머를 시작합니다.
    /// Rock.Mine() 내부에서 자동 호출되므로 외부에서 직접 부를 필요 없습니다.
    /// </summary>
    public void ReturnRock(Rock rock)
    {
        StartCoroutine(RespawnAfterDelay(rock, _respawnDelay));
    }

    private IEnumerator RespawnAfterDelay(Rock rock, float delay)
    {
        yield return new WaitForSeconds(delay);
        rock.Respawn();
    }

    // ──────────────────────────────────────────────
    //  조회 API (기존 MiningSystem 호환 유지)
    // ──────────────────────────────────────────────

    /// <summary>
    /// 지정 월드 좌표 목록에 가장 가까운 Available Rock 을 반환합니다.
    /// MiningSystem 에서 Spade 범위 좌표를 넘겨줄 때 사용합니다.
    /// </summary>
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

    /// <summary>특정 반경 안의 모든 Available Rock 반환 (탐지용)</summary>
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