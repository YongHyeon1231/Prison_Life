using DG.Tweening;
using UnityEngine;

/// <summary>
/// GetStar 오브젝트에 붙이는 컴포넌트.
/// Player가 트리거 위에 서면 StarPile의 Star를 하나씩 빠르게 Player 등 뒤로 날려 쌓습니다.
///
/// Inspector 연결
///   _starPile      : InfoDesk/GetStar의 StarPile
///   _jumpPower     : 높이 (높을수록 포물선이 큼)
///   _jumpDuration  : 비행 시간 (짧을수록 빠름)
/// </summary>
[RequireComponent(typeof(PlayerInteraction))]
public class GetStarInteraction : MonoBehaviour
{
    [SerializeField] private StarPile _starPile;

    [Header("Animation")]
    [SerializeField] private float _jumpPower    = 10f;
    [SerializeField] private float _jumpDuration = 0.12f;

    private void Start()
    {
        PlayerInteraction interaction = GetComponent<PlayerInteraction>();
        interaction.InteractInterval    = 0.05f;
        interaction.OnPlayerInteraction += OnPickup;
    }

    private void OnPickup(PlayerController player)
    {
        if (_starPile == null || _starPile.ObjectCount == 0) return;

        PlayerInventoryStack starStack = player.StarStack;
        if (starStack == null || starStack.IsFull) return;

        GameObject star = _starPile.RemoveFromPile();
        if (star == null) return;

        Vector3 dest = starStack.transform.position
                     + Vector3.up * starStack.TotalCount * 0.4f;

        star.transform
            .DOJump(dest, _jumpPower, 1, _jumpDuration)
            .OnComplete(() => starStack.AddToStack(star.transform));
    }
}
