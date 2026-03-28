using UnityEngine;

[RequireComponent(typeof(INetAuthority), typeof(INetActivator))]
public abstract class NetAwareBehavior : MonoBehaviour, INetAware
{
    private INetActivator _netActivator;
    protected INetActivator NetActivator => _netActivator ??= GetComponent<INetActivator>();

    protected virtual void OnEnable()
    {
        NetActivator.TryActivate(this);
    }

    protected virtual void OnDisable()
    {
        NetActivator.TryDeactivate(this);
    }

    public abstract void ActivateAt(INetAuthority authority);

    public abstract void DeactivateAt(INetAuthority authority);
}
