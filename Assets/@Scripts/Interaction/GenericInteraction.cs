using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 특정 타입의 캐릭터(T)가 트리거에 진입/이탈/체류할 때 이벤트를 발행하는 제네릭 베이스.
/// PlayerInteraction, GuestInteraction 이 이를 상속합니다.
/// </summary>
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
