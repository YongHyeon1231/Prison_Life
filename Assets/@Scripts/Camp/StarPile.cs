using UnityEngine;

public class StarPile : AnimatedPile
{
    [SerializeField] private GameObject _starPrefab;

    private void Awake()
    {
        _itemPrefab = _starPrefab;
    }
}
