using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PlayerInteraction : MonoBehaviour
{
    public Action<PlayerController> OnPlayerInteraction;
    public Action<PlayerController> OnPlayerEntered;
    public Action<PlayerController> OnPlayerExited;

    public float InteractInterval = 0.5f;

    public bool IsPlayerPresent => _player != null;

    private PlayerController _player;

    private void Start()
    {
        StartCoroutine(CoPlayerInteraction());
    }

    private IEnumerator CoPlayerInteraction()
    {
        while (true)
        {
            yield return new WaitForSeconds(InteractInterval);

            if (_player != null)
                OnPlayerInteraction?.Invoke(_player);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc == null) return;

        _player = pc;
        OnPlayerEntered?.Invoke(pc);
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc == null) return;

        OnPlayerExited?.Invoke(_player);
        _player = null;
    }
}
