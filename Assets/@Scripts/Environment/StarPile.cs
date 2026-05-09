using UnityEngine;

/// <summary>
/// 별 보상 아이템을 쌓는 파일.
/// _starPrefab을 Inspector에서 연결하면 _itemPrefab으로 전달됩니다.
/// </summary>
public class StarPile : AnimatedPile
{
    [SerializeField] private GameObject _starPrefab;

    private void Awake()
    {
        _itemPrefab = _starPrefab;
    }
}
