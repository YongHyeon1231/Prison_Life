using UnityEngine;

/// <summary>
/// Rock 오브젝트.
/// Destroy 대신 SetActive(false) 후 RockPool 에 반환합니다.
/// RockPool 이 일정 시간 뒤 원래 위치에 다시 활성화(리스폰)합니다.
///
/// Inspector 설정:
///   - BoxCollider, Is Trigger = true  (플레이어가 통과 가능)
///   - Rigidbody, Is Kinematic = true  (Trigger↔Trigger 감지에 필요)
///   - Tag : "Rock"
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Rock : MonoBehaviour
{
    private void Awake()
    {
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity  = false;
    }

    // ──────────────────────────────────────────────
    //  상태
    // ──────────────────────────────────────────────

    /// <summary>채굴 가능한 상태인지 여부 (비활성 = false)</summary>
    public bool IsAvailable => gameObject.activeSelf;

    /// <summary>스폰/리스폰 기준 위치 (RockPool 이 설정)</summary>
    public Vector3 OriginPosition { get; set; }

    // ──────────────────────────────────────────────
    //  채굴
    // ──────────────────────────────────────────────

    /// <summary>
    /// WeaponCollider 에서 호출합니다.
    /// 오브젝트를 비활성화하고 RockPool 에 반환합니다.
    /// </summary>
    public void Mine()
    {
        if (!IsAvailable) return;

        gameObject.SetActive(false);
        RockPool.Instance.ReturnRock(this);
    }

    // ──────────────────────────────────────────────
    //  리스폰
    // ──────────────────────────────────────────────

    /// <summary>RockPool 이 호출 — 원래 위치에 다시 활성화합니다.</summary>
    public void Respawn()
    {
        transform.position = OriginPosition;
        transform.rotation = Quaternion.identity;
        gameObject.SetActive(true);
    }
}