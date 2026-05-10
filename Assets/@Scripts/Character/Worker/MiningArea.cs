using UnityEngine;

/// <summary>
/// 씬에 하나 배치해 Mining Worker의 줄별 패트롤 경로와 Rock 배달 목적지를 제공합니다.
/// MiningWorkerController가 Start()에서 RegisterWorker()를 호출해 자기 줄을 순서대로 받아갑니다.
///
/// Inspector 설정:
///   _lanes      : Worker 수만큼 PatrolLane 설정 (각 줄의 A·B 끝 지점)
///   _targetPile : MachineStandInput 하위 PutDownTheRockPile (전 줄 공유)
/// </summary>
public class MiningArea : MonoBehaviour
{
    [System.Serializable]
    public struct PatrolLane
    {
        public Transform patrolA;
        public Transform patrolB;
    }

    [SerializeField] private PatrolLane[]       _lanes;
    [SerializeField] private PutDownTheRockPile _targetPile;

    public PutDownTheRockPile TargetPile => _targetPile;

    private int _nextLaneIndex;

    /// <summary>
    /// Worker가 소환 순서대로 호출합니다.
    /// 남은 줄이 있으면 패트롤 양끝 지점을 반환하고 true, 없으면 false를 반환합니다.
    /// </summary>
    public bool TryGetNextLane(out Transform patrolA, out Transform patrolB)
    {
        if (_nextLaneIndex < _lanes.Length)
        {
            patrolA = _lanes[_nextLaneIndex].patrolA;
            patrolB = _lanes[_nextLaneIndex].patrolB;
            _nextLaneIndex++;
            return true;
        }

        patrolA = patrolB = null;
        return false;
    }
}
