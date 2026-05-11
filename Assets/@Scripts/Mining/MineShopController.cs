using UnityEngine;

public class MineShopController : MonoBehaviour
{
    [SerializeField] private GameObject _playerLevelUpArea;
    [SerializeField] private GameObject _mineWorkerShop;

    public void OpenPlayerLevelUpArea()
    {
        if (_playerLevelUpArea != null)
            _playerLevelUpArea.SetActive(true);
    }

    public void OpenMineWorkerShop()
    {
        if (_mineWorkerShop != null)
            _mineWorkerShop.SetActive(true);
    }
}
