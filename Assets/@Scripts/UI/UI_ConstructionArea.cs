using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ConstructionArea : UI_BaseConstructionArea
{
    [Header("UI")]
    [SerializeField] private Image           _fillImage;
    [SerializeField] private TextMeshProUGUI _costText;

    [Header("Upgrade")]
    [SerializeField] private int _upgradedAmount = 50;

    [Header("Resource Animation")]
    [SerializeField] private float _flyPower    = 5f;
    [SerializeField] private float _flyDuration = 0.3f;

    protected override void Start()
    {
        base.Start();

        if (_fillImage != null) _fillImage.fillAmount = 0f;
        if (_costText  != null) _costText.text = _requiredAmount.ToString();
    }

    protected override void OnProgressStep(float delta, float progress, GameObject visualItem)
    {
        if (_fillImage != null) _fillImage.fillAmount = progress;

        if (visualItem != null)
            ResourceVisualMover.FlyToTarget(
                visualItem.transform,
                transform.position,
                _flyPower,
                _flyDuration);
    }

    protected override void OnComplete(PlayerController player)
    {
        player.UpgradeWeaponLevel();

        _requiredAmount = _upgradedAmount;
        _progress       = 0f;
        _isComplete     = false;

        if (_fillImage != null) _fillImage.fillAmount = 0f;
        if (_costText  != null) _costText.text = _requiredAmount.ToString();
    }
}
