using UnityEngine;
using static Define;

[RequireComponent(typeof(PlayerInteraction))]
public abstract class UI_BaseConstructionArea : MonoBehaviour
{
    [Header("Construction")]
    [SerializeField] protected ResourceType _requiredResource = ResourceType.Star;
    [SerializeField] protected int          _requiredAmount   = 20;

    protected float _progress   = 0f;
    protected bool  _isComplete = false;

    private PlayerInteraction _interaction;

    protected virtual void Awake()
    {
        _interaction = GetComponent<PlayerInteraction>();
    }

    protected virtual void Start()
    {
        _interaction.InteractInterval    = 0.1f;
        _interaction.OnInteraction += OnPlayerTick;
    }

    private void OnPlayerTick(PlayerController player)
    {
        if (_isComplete) return;
        if (GameManager.Instance.Inventory.GetResourceCount(_requiredResource) <= 0) return;

        GameObject item = player.TakeResourceItem(_requiredResource);
        if (item == null) return;

        GameManager.Instance.Sound.Play(SoundType.ItemPutDown);
        _progress = Mathf.Clamp01(_progress + 1f / _requiredAmount);
        OnProgressStep(1f / _requiredAmount, _progress, item);

        if (_progress >= 1f)
        {
            _isComplete = true;
            GameManager.Instance.Sound.Play(SoundType.Purchase);
            OnComplete(player);
        }
    }

    protected abstract void OnProgressStep(float delta, float progress, GameObject visualItem);
    protected abstract void OnComplete(PlayerController player);
}
