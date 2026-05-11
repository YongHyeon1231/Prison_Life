using UnityEngine;

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
