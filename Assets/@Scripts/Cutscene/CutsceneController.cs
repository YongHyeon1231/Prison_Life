using System.Collections;
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

    public void PlayMineShopOpen(PlayerController player)
    {
        StartCoroutine(CoMineShopOpen(player));
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
