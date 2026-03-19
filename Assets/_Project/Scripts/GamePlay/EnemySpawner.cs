using UnityEngine;
using UnityEngine.Pool;
using Unity.Netcode;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    private IObjectPool<GameObject> _pool;

    public override void OnNetworkSpawn()
    {
        // 서버(Host)에서만 적 생성 로직을 관리합니다.
        if (!IsServer) return;

        _pool = new ObjectPool<GameObject>(
            createFunc: () => {
                var obj = Instantiate(enemyPrefab);
                // 네트워크 오브젝트로 스폰 시켜야 모든 클라이언트에게 보입니다.
                obj.GetComponent<NetworkObject>().Spawn();
                return obj;
            },
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            defaultCapacity: 50,
            maxSize: 200
        );

        // 테스트용: 2초마다 적 생성
        InvokeRepeating(nameof(SpawnFromPool_Test), 2f, 2f);
    }

    private void SpawnFromPool_Test()
    {
        var enemy = _pool.Get();
        // 플레이어 근처 랜덤 위치에 배치 (지금은 단순 랜덤)
        enemy.transform.position = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
    }
}