using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Rock : MonoBehaviour
{
    private void Awake()
    {
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity  = false;
    }

    public bool IsAvailable => gameObject.activeSelf;

    private int _hitCount = 0;

    public bool TryHit(int requiredHits = 2)
    {
        if (!IsAvailable) return false;
        _hitCount++;
        return _hitCount >= requiredHits;
    }

    public Vector3 OriginPosition { get; set; }

    public void Mine()
    {
        if (!IsAvailable) return;

        gameObject.SetActive(false);
        RockPool.Instance.ReturnRock(this);
    }

    public void Respawn()
    {
        _hitCount          = 0;
        transform.position = OriginPosition;
        transform.rotation = Quaternion.identity;
        gameObject.SetActive(true);
    }
}
