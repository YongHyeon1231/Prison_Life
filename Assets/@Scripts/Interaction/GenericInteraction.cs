using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public abstract class GenericInteraction<T> : MonoBehaviour where T : MonoBehaviour
{
    public Action<T> OnInteraction;
    public Action<T> OnEntered;
    public Action<T> OnExited;

    public float InteractInterval = 0.5f;

    public bool IsPresent => _target != null;

    protected T _target;

    private void Start()
    {
        StartCoroutine(CoInteraction());
    }

    private IEnumerator CoInteraction()
    {
        while (true)
        {
            yield return new WaitForSeconds(InteractInterval);
            if (_target != null)
                OnInteraction?.Invoke(_target);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        T target = other.GetComponent<T>();
        if (target == null) return;
        _target = target;
        OnEntered?.Invoke(target);
    }

    private void OnTriggerExit(Collider other)
    {
        T target = other.GetComponent<T>();
        if (target == null) return;
        OnExited?.Invoke(_target);
        _target = null;
    }
}
