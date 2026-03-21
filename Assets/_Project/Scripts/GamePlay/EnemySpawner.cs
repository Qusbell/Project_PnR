using UnityEngine;
using UnityEngine.Pool;
using Unity.Netcode;

public class EnemySpawner : NetworkBehaviour
{
    [field: SerializeField]
    private GameObject EnemyPrefab { get; set; }

    private IObjectPool<GameObject> _pool;
    private IObjectPool<GameObject> Pool => _pool ??= new ObjectPool<GameObject>(
            createFunc: CreateEnemy_Test,
            actionOnGet: (obj) => { obj.SetActive(true); },
            actionOnRelease: (obj) => { obj.SetActive(false); },
            defaultCapacity: 50,
            maxSize: 200);

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }

        InvokeRepeating(nameof(SpawnFromPool_Test), 2f, 2f);
    }

    private GameObject CreateEnemy_Test()
    {
        GameObject obj = Instantiate(EnemyPrefab);
        // 인터페이스 참조 원칙에 따라 NetworkObject도 프로퍼티화 가능하나 
        // 여기서는 컴포넌트 취득 후 스폰 규약 준수
        if (obj.TryGetComponent(out NetworkObject netObj))
        {
            netObj.Spawn();
        }
        return obj;
    }

    private void SpawnFromPool_Test()
    {
        GameObject enemy = Pool.Get();
        enemy.transform.position = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
    }


    private void OnValidate()
    {
        if(EnemyPrefab == null) { Debug.LogError($"{name} : EnemyPrefab = null"); }
    }

}
