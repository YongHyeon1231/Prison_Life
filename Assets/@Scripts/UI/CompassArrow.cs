using UnityEngine;

/// <summary>
/// Player에 부착.
/// _visual 위치는 플레이어 기준 (0, -1.2, 0) 고정.
/// Rotation X=90, Z=0 고정, Y만 목표 방향으로 회전.
/// </summary>
public class CompassArrow : MonoBehaviour
{
    [SerializeField] private GameObject _visual;

    [Header("Distance")]
    [SerializeField] private float _hideDistance = 5f;

    private static readonly Vector3 LocalOffset = new(0f, -1.2f, 0f);

    private void LateUpdate()
    {
        if (_visual == null) return;

        TutorialGuide guide = TutorialGuide.Instance;

        if (guide == null || !guide.HasActiveArrow)
        {
            SetVisible(false);
            return;
        }

        Vector3 targetPos = guide.ArrowTargetPosition;
        float   dx        = targetPos.x - transform.position.x;
        float   dz        = targetPos.z - transform.position.z;
        float   dist      = Mathf.Sqrt(dx * dx + dz * dz);

        if (dist <= _hideDistance)
        {
            SetVisible(false);
            return;
        }

        SetVisible(true);

        float angleY = Mathf.Atan2(dx, dz) * Mathf.Rad2Deg;

        _visual.transform.localPosition = LocalOffset;
        _visual.transform.rotation      = Quaternion.Euler(90f, angleY, 0f);
    }

    private void SetVisible(bool visible)
    {
        if (_visual.activeSelf != visible)
            _visual.SetActive(visible);
    }
}
