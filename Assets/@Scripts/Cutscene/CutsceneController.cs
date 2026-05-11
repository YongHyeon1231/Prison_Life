using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CutsceneController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private Animator         _mineShopAnimator;

    [Header("MineShop Cutscene")]
    [SerializeField] private Vector3 _mineShopCamTarget = new(-4.28f, 6.5f, -9.10f);
    [SerializeField] private float   _camMoveDuration   = 1.5f;

    [Header("Camp Full Cutscene")]
    [SerializeField] private Vector3    _campCamTarget       = new(-13.6f, 6.5f, -11.83f);
    [SerializeField] private GameObject _campUpgradeArea;
    [SerializeField] private float      _campFullDelay       = 2f;
    [SerializeField] private float      _campCamMoveDuration = 1.5f;
    [SerializeField] private float      _campCamWaitDuration = 7f;

    [Header("Camp Upgrade Cutscene")]
    [SerializeField] private Vector3           _campUpgradeCamTarget       = new(-13.6f, 6.5f, -11.83f);
    [SerializeField] private float             _campUpgradeCamMoveDuration = 1.5f;
    [SerializeField] private float             _campUpgradeCamWaitDuration = 3f;
    [SerializeField] private List<GameObject>  _campUpgradeReturnObjects;

    public void PlayCampUpgrade(Action onComplete)
    {
        StartCoroutine(CoCampUpgrade(onComplete));
    }

    public void PlayMineShopOpen(PlayerController player)
    {
        StartCoroutine(CoMineShopOpen(player));
    }

    public void PlayCampFull()
    {
        StartCoroutine(CoCampFull());
    }

    private IEnumerator CoCampUpgrade(Action onComplete)
    {
        PlayerController player = GameManager.Instance.Player;
        if (player != null) player.SetLocked(true);

        bool camReady = false;
        _cameraController.MoveCameraXZ(_campUpgradeCamTarget, _campUpgradeCamMoveDuration, () => camReady = true);
        yield return new WaitUntil(() => camReady);

        onComplete?.Invoke();

        yield return new WaitForSeconds(_campUpgradeCamWaitDuration);

        bool camBack = false;
        _cameraController.ReturnToFollow(_campUpgradeCamMoveDuration, () => camBack = true);
        yield return new WaitUntil(() => camBack);

        if (_campUpgradeReturnObjects != null)
            foreach (var obj in _campUpgradeReturnObjects)
                if (obj != null) obj.SetActive(true);

        GameManager.Instance.Sound.Play(SoundType.OpenAD);

        if (player != null) player.SetLocked(false);
    }

    private IEnumerator CoCampFull()
    {
        PlayerController player = GameManager.Instance.Player;
        if (player != null) player.SetLocked(true);

        yield return new WaitForSeconds(_campFullDelay);

        bool camReady = false;
        _cameraController.MoveCameraXZ(_campCamTarget, _campCamMoveDuration, () => camReady = true);
        yield return new WaitUntil(() => camReady);

        if (_campUpgradeArea != null) _campUpgradeArea.SetActive(true);

        yield return new WaitForSeconds(_campCamWaitDuration);

        bool camBack = false;
        _cameraController.ReturnToFollow(_campCamMoveDuration, () => camBack = true);
        yield return new WaitUntil(() => camBack);

        if (player != null) player.SetLocked(false);
    }

    private IEnumerator CoMineShopOpen(PlayerController player)
    {
        player.SetLocked(true);

        // 카메라를 타겟으로 이동 (X·Z만)
        bool camReady = false;
        _cameraController.MoveCameraXZ(_mineShopCamTarget, _camMoveDuration, () => camReady = true);
        yield return new WaitUntil(() => camReady);

        // 애니메이션 재생
        _mineShopAnimator.Play(MINE_SHOP_FIRST_OPEN);
        yield return null; // 상태 갱신 대기

        float animLength = _mineShopAnimator.GetCurrentAnimatorStateInfo(0).length;
        float timer = 0f;
        while (timer < animLength)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // 카메라 복귀
        bool camBack = false;
        _cameraController.ReturnToFollow(_camMoveDuration, () => camBack = true);
        yield return new WaitUntil(() => camBack);

        player.SetLocked(false);
    }
}
