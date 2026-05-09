using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GuestInteraction : MonoBehaviour
{
    public Action<GuestController> OnGuestInteraction;
    public Action<GuestController> OnGuestEntered;
    public Action<GuestController> OnGuestExited;

    public float InteractInterval = 0.5f;

    public bool IsGuestPresent => _guest != null;

    private GuestController _guest;

    private void Start()
    {
        StartCoroutine(CoGuestInteraction());
    }

    private IEnumerator CoGuestInteraction()
    {
        while (true)
        {
            yield return new WaitForSeconds(InteractInterval);

            if (_guest != null)
                OnGuestInteraction?.Invoke(_guest);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GuestController gc = other.GetComponent<GuestController>();
        if (gc == null) return;
        _guest = gc;
        OnGuestEntered?.Invoke(gc);
    }

    private void OnTriggerExit(Collider other)
    {
        GuestController gc = other.GetComponent<GuestController>();
        if (gc == null) return;

        OnGuestExited?.Invoke(_guest);
        _guest = null;
    }
}
