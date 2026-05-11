using UnityEngine;

public class UI_WorkerPurchaseArea : UI_WorkerShopAreaBase
{
    [SerializeField] private Waypoints  _spawnPoints;
    [SerializeField] private GameObject _nextShop;

    protected override void SpawnWorkers()
    {
        GameObject prefab = GameManager.Instance.GetWorkerPrefab(_workerType);
        if (prefab == null || _spawnPoints == null) return;
        foreach (Transform point in _spawnPoints.GetPoints())
            Instantiate(prefab, point.position, point.rotation);
    }

    protected override void OnBeforeDeactivate()
    {
        if (_nextShop != null) _nextShop.SetActive(true);
    }
}
