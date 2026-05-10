using System;
using DG.Tweening;
using UnityEngine;

public static class ResourceVisualMover
{
    public static void FlyToTarget(
        Transform item,
        Vector3   destination,
        float     jumpPower = 5f,
        float     duration  = 0.3f,
        Action    onArrived = null)
    {
        item.SetParent(null);
        item.DOJump(destination, jumpPower, 1, duration)
            .OnComplete(() =>
            {
                onArrived?.Invoke();
                UnityEngine.Object.Destroy(item.gameObject);
            });
    }
}
