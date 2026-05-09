using TMPro;
using UnityEngine;

/// <summary>
/// 게스트 머리 위 주문 말풍선 UI.
/// Billboard 컴포넌트가 항상 카메라를 향하도록 설정되어 있어야 합니다.
/// CountText는 Inspector에서 연결하세요.
/// </summary>
public class UI_OrderBubble : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _countText;

    public void SetCount(int count)
    {
        if (_countText != null)
            _countText.text = $"x {count}";
    }
}
