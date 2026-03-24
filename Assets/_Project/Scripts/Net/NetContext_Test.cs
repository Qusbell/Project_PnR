using System;
using System.Collections.Generic;
using Unity.Netcode;


/// <summary>
/// INetAware들의 초기화 담당
/// </summary>
public class NetContext_Test : NetworkBehaviour, INetAuthority//, INetContext
{
    // ==== Field ==== //

    private HashSet<INetAware> Targets { get; set; } = new();

    /// <summary>
    /// 스폰 상태인가 + 활성화 상태인가
    /// </summary>
    private bool ShouldBeActive => IsSpawned && enabled;

    


    // ==== Life Cycle ==== //

    private void OnEnable()
    {
        RefreshState();
    }

    public override void OnNetworkSpawn()
    {
        RefreshState();
    }

    private void OnDisable()
    {
        RefreshState();
    }

    public override void OnNetworkDespawn()
    {
        RefreshState();
    }



    // ==== Interface ==== //

    public void TryActivate(INetAware target)
    {
        if (!Targets.Add(target)) { return; }

        if (ShouldBeActive)
        {
            target.ActivateAt(this);
        }
    }


    // ==== Custom ==== //


    private void RefreshState()
    {
        if (ShouldBeActive) { ActivateAll(); }
        else                { DeactivatedAll(); }
    }

    public void ActivateAll()
    {
        foreach (var target in Targets)
        {
            target.ActivateAt(this);
        }
    }

    public void DeactivatedAll()
    {
        foreach (var target in Targets)
        {
            target.DeactivateAt(this);
        }
    }

}
