using System;
using Unity.Netcode;

public class NetContext_Test2 : NetworkBehaviour, INetAuthority, INetActivator
{
    private event Action<INetAuthority> OnActivateReserved;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        OnActivateReserved?.Invoke(this);
        OnActivateReserved = null;
    }

    public void TryActivate(INetAware self)
    {
        // 이미 스폰된 상태라면 즉시 실행
        if (IsSpawned)
        {
            self.ActivateAt(this);
        }

        // 스폰이 아직 미처 되지 않았다면 예약
        else
        {
            OnActivateReserved -= self.ActivateAt;
            OnActivateReserved += self.ActivateAt;
        }
    }

    public void TryDeactivate(INetAware self)
    {
        // 예약 리스트에서 제거 (스폰 전 비활성화 요청 대비)
        OnActivateReserved -= self.ActivateAt;

        // 호출된 즉시 제거
        // Activate는 아직 INetAuthority가 초기화 이전이므로 예약이 필요하지만,
        // Deactivate는 이미 Spawn 이후의 상태라 볼 수 있으므로, 어떤 상태이든 Deactivate시킴
        self.DeactivateAt(this);
    }


}
