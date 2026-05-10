using UnityEngine;

/// <summary>
/// Camp_Floor 오브젝트에 붙이는 트리거 감지기.
/// Guest가 CampGround 위에 올라서면 부모의 CampController에 알립니다.
/// </summary>
[RequireComponent(typeof(Collider))]
public class CampFloorZone : MonoBehaviour
{
    private CampController _camp;

    private void Awake()
    {
        _camp = GetComponentInParent<CampController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        GuestController gc = other.GetComponent<GuestController>();
        if (gc == null) return;

        _camp.OnGuestEnteredFloor(gc);
    }

    private void OnTriggerExit(Collider other)
    {
        GuestController gc = other.GetComponent<GuestController>();
        if (gc == null) return;

        _camp.OnGuestExitedFloor(gc);
    }
}
