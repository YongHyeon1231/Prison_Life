using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(PlayerInteraction))]
public class GetStarInteraction : MonoBehaviour
{
    [SerializeField] private StarPile          _starPile;
    [SerializeField] private CutsceneController _cutsceneController;

    [Header("Animation")]
    [SerializeField] private float _jumpPower    = 10f;
    [SerializeField] private float _jumpDuration = 0.12f;

    private bool _firstStarTriggered = false;

    private void Start()
    {
        PlayerInteraction interaction = GetComponent<PlayerInteraction>();
        interaction.InteractInterval    = 0.05f;
        interaction.OnInteraction += OnPickup;
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
            .OnComplete(() =>
            {
                starStack.AddToStack(star.transform);
                GameManager.Instance.Sound.Play(Define.SoundType.GetStar);

                if (!_firstStarTriggered && _cutsceneController != null)
                {
                    _firstStarTriggered = true;
                    _cutsceneController.PlayMineShopOpen(player);
                }
            });
    }
}
