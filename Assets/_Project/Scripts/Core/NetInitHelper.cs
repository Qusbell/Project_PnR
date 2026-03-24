using UnityEngine;

public class NetInitHelper
{
    private INetAware Owner { get; set; }
    private INetActivator Activator { get; set; }


    public NetInitHelper(INetAware owner, MonoBehaviour target)
    {
        Owner = owner;
        Activator = target.GetComponentInParent<INetActivator>();
    }

    public NetInitHelper(INetAware owner, INetActivator activator)
    {
        Owner = owner;
        Activator = activator;
    }


    public void Activate()
    {
        Activator?.TryActivate(Owner);
    }

    public void Deactivate()
    {
        Activator?.TryDeactivate(Owner);
    }
}
