using Unity.Netcode;
using UnityEngine;

public class NetContext_Test : NetworkBehaviour, INetAuthority
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        var netAwares = GetComponents<INetAware>();
        foreach(var netAware in netAwares)
        {
            netAware.Initialize(this);
        }
    }

}
