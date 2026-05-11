using UnityEngine;

public class UI_CounterWorkerShopArea : UI_WorkerShopAreaBase
{
    [SerializeField] private Transform _spawnPoint;

    protected override void SpawnWorkers()
    {
        GameObject prefab = GameManager.Instance.GetWorkerPrefab(_workerType);
        if (prefab == null || _spawnPoint == null) return;
        Instantiate(prefab, _spawnPoint.position, _spawnPoint.rotation);
    }
}
