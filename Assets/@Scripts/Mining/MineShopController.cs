using UnityEngine;

/// <summary>
/// MineShop 오브젝트에 부착합니다.
/// 각 애니메이션 클립에서 IsActive 키프레임 대신 Animation Event로 이 메서드들을 호출합니다.
///
/// 설정 방법:
///   공통) 각 클립에서 해당 오브젝트의 IsActive 키프레임 트랙을 삭제합니다.
///         활성화가 필요한 프레임에 Animation Event를 추가합니다.
///
///   MineShop_First_Open  → Animation Event: OpenPlayerLevelUpArea
///   MineShop_FullAmount  → Animation Event: OpenMineWorkerShop
/// </summary>
public class MineShopController : MonoBehaviour
{
    [SerializeField] private GameObject _playerLevelUpArea;
    [SerializeField] private GameObject _mineWorkerShop;

    /// <summary>MineShop_First_Open Animation Event에서 호출합니다.</summary>
    public void OpenPlayerLevelUpArea()
    {
        if (_playerLevelUpArea != null)
            _playerLevelUpArea.SetActive(true);
    }

    /// <summary>MineShop_FullAmount Animation Event에서 호출합니다.</summary>
    public void OpenMineWorkerShop()
    {
        if (_mineWorkerShop != null)
            _mineWorkerShop.SetActive(true);
    }
}
