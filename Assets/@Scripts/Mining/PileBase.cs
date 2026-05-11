using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PileBase : MonoBehaviour
{
    [Header("Pile Layout")]
    [SerializeField] private int     _row    = 2;
    [SerializeField] private int     _column = 2;
    [SerializeField] private Vector3 _size   = new Vector3(0.5f, 0.1f, 0.5f);

    protected Stack<GameObject> _objects = new Stack<GameObject>();

    public int ObjectCount => _objects.Count;

    public virtual void AddToPile(GameObject go)
    {
        _objects.Push(go);
        go.transform.position = GetPositionAt(_objects.Count - 1);
        go.transform.rotation = Quaternion.identity;
        go.transform.SetParent(transform);
    }

    public virtual GameObject RemoveFromPile()
    {
        if (_objects.Count == 0) return null;

        GameObject go = _objects.Pop();
        go.transform.SetParent(null);
        return go;
    }

    public virtual void ClearPile()
    {
        while (_objects.Count > 0)
        {
            GameObject go = _objects.Pop();
            if (go != null) Destroy(go);
        }
    }

    protected Vector3 GetPositionAt(int pileIndex)
    {
        Vector3 offset   = new Vector3((_row - 1) * _size.x / 2f, 0f, (_column - 1) * _size.z / 2f);
        Vector3 startPos = transform.position - offset;

        int col    = pileIndex % _row;
        int row    = (pileIndex / _row) % _column;
        int height = pileIndex / (_row * _column);

        float x = startPos.x + col    * _size.x;
        float y = startPos.y + height * _size.y;
        float z = startPos.z + row    * _size.z;

        return new Vector3(x, y, z);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color  = Color.yellow;

        Vector3 offset   = new Vector3((_row - 1) * _size.x / 2f, 0f, (_column - 1) * _size.z / 2f);
        Vector3 startPos = -offset;

        for (int r = 0; r < _row; r++)
        {
            for (int c = 0; c < _column; c++)
            {
                Vector3 center = startPos + new Vector3(r * _size.x, _size.y / 2f, c * _size.z);
                Gizmos.DrawWireCube(center, _size);
            }
        }

        Gizmos.matrix = Matrix4x4.identity;
    }
#endif
}
