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

    public void TryActivate(INetAware target)
    {
        // 이미 스폰된 상태라면 즉시 실행
        if (IsSpawned)
        {
            target.ActivateAt(this);
        }

        // 스폰이 아직 미처 되지 않았다면 예약
        else
        {
            OnActivateReserved -= target.ActivateAt;
            OnActivateReserved += target.ActivateAt;
        }
    }

    public void TryDeactivate(INetAware target)
    {
        // 예약 리스트에서 제거 (스폰 전 비활성화 요청 대비)
        OnActivateReserved -= target.ActivateAt;

        // 스폰된 상태면 제거 (스폰 상태조차 아니면 별 의미 없음)
        if (IsSpawned)
        {
            target.DeactivateAt(this);
        }
    }


}
