using System;
using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _target;

    private Vector3 _offset;
    private bool    _isFollowing = true;

    void Start()
    {
        _offset = transform.position - _target.position;
    }

    void LateUpdate()
    {
        if (!_isFollowing) return;
        transform.position = _offset + _target.position;
    }

    public void MoveCameraXZ(Vector3 destination, float duration, Action onComplete = null)
    {
        _isFollowing = false;
        transform.DOKill();

        Vector3 dest = new Vector3(destination.x, transform.position.y, destination.z);
        transform.DOMove(dest, duration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => onComplete?.Invoke());
    }

    public void ReturnToFollow(float duration, Action onComplete = null)
    {
        transform.DOKill();

        Vector3 followPos = _offset + _target.position;
        Vector3 dest = new Vector3(followPos.x, transform.position.y, followPos.z);
        transform.DOMove(dest, duration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                _isFollowing = true;
                onComplete?.Invoke();
            });
    }
}
