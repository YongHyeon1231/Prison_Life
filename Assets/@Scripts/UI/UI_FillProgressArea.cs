using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class UI_FillProgressArea : UI_BaseConstructionArea
{
    [Header("UI")]
    [SerializeField] protected Image           _fillImage;
    [SerializeField] protected TextMeshProUGUI _costText;

    [Header("Resource Animation")]
    [SerializeField] protected float _flyPower    = 5f;
    [SerializeField] protected float _flyDuration = 0.3f;

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
            ResourceVisualMover.FlyToTarget(visualItem.transform, transform.position, _flyPower, _flyDuration);
    }

    protected void ResetFill(int newAmount = -1)
    {
        if (newAmount > 0) _requiredAmount = newAmount;
        _progress   = 0f;
        _isComplete = false;
        if (_fillImage != null) _fillImage.fillAmount = 0f;
        if (_costText  != null) _costText.text = _requiredAmount.ToString();
    }
}
