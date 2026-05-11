using UnityEngine;

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
