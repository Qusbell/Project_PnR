using UnityEngine;

/// <summary>
/// 현재 GameObject가 Net상에서 활성화되어야 하는지 여부를 판단하여, <br/>
/// 활성화/비활성화 시점에 맞게 ActivateAt/DeactivateAt 메서드를 호출하는 MonoBehaviour <br/>
/// </summary>
[RequireComponent(typeof(INetContext), typeof(INetActivateProxy))]
public abstract class NetAwareBehavior : MonoBehaviour, INetAware
{
    private INetActivateProxy _netActivator;
    protected INetActivateProxy NetActivator => _netActivator ??= GetComponent<INetActivateProxy>();

    protected virtual void OnEnable()
    {
        NetActivator.TryActivate(this);
    }

    protected virtual void OnDisable()
    {
        NetActivator.TryDeactivate(this);
    }

    public abstract void ActivateAt(INetContext authority);

    public abstract void DeactivateAt(INetContext authority);
}
