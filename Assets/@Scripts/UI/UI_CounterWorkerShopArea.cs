using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

/// <summary>
/// Star를 소비해 Fill을 한 번 채우면 지정 위치에 Worker_Counter를 스폰하고 비활성화됩니다.
/// Mine_Worker_Shop(UI_WorkerPurchaseArea)의 _nextShop에 이 오브젝트를 연결하면
/// Mine_Worker_Shop 완료 직후 자동으로 활성화됩니다.
///
/// Inspector 설정:
///   _fillImage  : 프로그레스 바 Image
///   _costText   : 필요 Star 수 표시 TMP
///   _workerType : 스폰할 Worker 종류 (기본값 Counter)
///   _spawnPoint : Worker가 등장할 위치 Transform
/// Worker 프리팹 : GameManager > Worker Prefabs 섹션에 타입별 등록
/// </summary>
public class UI_CounterWorkerShopArea : UI_BaseConstructionArea
{
    [Header("UI")]
    [SerializeField] private Image           _fillImage;
    [SerializeField] private TextMeshProUGUI _costText;

    [Header("Resource Animation")]
    [SerializeField] private float _flyPower    = 5f;
    [SerializeField] private float _flyDuration = 0.3f;

    [Header("Worker Spawn")]
    [SerializeField] private WorkerType _workerType = WorkerType.Counter;
    [SerializeField] private Transform  _spawnPoint;

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
        SpawnWorker();
        StartCoroutine(DeactivateAfterDelay(0.5f));
    }

    private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    private void SpawnWorker()
    {
        GameObject prefab = GameManager.Instance.GetWorkerPrefab(_workerType);
        if (prefab == null) return;

        if (_spawnPoint == null)
        {
            Debug.LogWarning("[UI_CounterWorkerShopArea] Spawn Point가 설정되지 않았습니다.");
            return;
        }

        Instantiate(prefab, _spawnPoint.position, _spawnPoint.rotation);
    }
}
