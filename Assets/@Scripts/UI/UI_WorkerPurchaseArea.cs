using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

/// <summary>
/// Star를 소비해 Fill을 한 번 채우면 Waypoints 위치에 Worker를 동시 스폰하고 비활성화됩니다.
/// 업그레이드 사이클 없는 단발성 구매 영역입니다.
///
/// Inspector 설정:
///   _fillImage   : 프로그레스 바 Image
///   _costText    : 필요 Star 수 표시 TMP
///   _workerType  : 스폰할 Worker 종류 (Mining / Counter)
///   _spawnPoints : Worker가 등장할 위치들을 담은 Waypoints 오브젝트
/// Worker 프리팹 : GameManager > Worker Prefabs 섹션에 타입별 등록
/// </summary>
public class UI_WorkerPurchaseArea : UI_BaseConstructionArea
{
    [Header("UI")]
    [SerializeField] private Image           _fillImage;
    [SerializeField] private TextMeshProUGUI _costText;

    [Header("Resource Animation")]
    [SerializeField] private float _flyPower    = 5f;
    [SerializeField] private float _flyDuration = 0.3f;

    [Header("Worker Spawn")]
    [SerializeField] private WorkerType _workerType  = WorkerType.Mining;
    [SerializeField] private Waypoints  _spawnPoints;

    [Header("Chain")]
    [SerializeField] private GameObject _nextShop;

    [Header("Tutorial")]
    [SerializeField] private bool _completesTutorialStep;

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
        SpawnWorkers();
        if (_completesTutorialStep && TutorialGuide.Instance != null) TutorialGuide.Instance.CompleteCurrentStep();
        StartCoroutine(DeactivateAfterDelay(0.5f));
    }

    private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_nextShop != null) _nextShop.SetActive(true);
        gameObject.SetActive(false);
    }

    private void SpawnWorkers()
    {
        GameObject prefab = GameManager.Instance.GetWorkerPrefab(_workerType);
        if (prefab == null) return;

        if (_spawnPoints == null)
        {
            Debug.LogWarning("[UI_WorkerPurchaseArea] Spawn Points가 설정되지 않았습니다.");
            return;
        }

        foreach (Transform point in _spawnPoints.GetPoints())
            Instantiate(prefab, point.position, point.rotation);
    }
}
