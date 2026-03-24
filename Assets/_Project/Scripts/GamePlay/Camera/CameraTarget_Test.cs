using UnityEngine;

public class CameraTarget_Test : MonoBehaviour, IDestination, INetAware
{
    private INetActivator _initilaizer;
    private INetActivator NetInit => _initilaizer ??= GetComponentInParent<INetActivator>();


    public Vector2 Position => transform.position;

    public bool IsActivated => enabled;

    private ITraveler targetCamera;
    private ITraveler TargetCamera => targetCamera ??= Camera.main?.GetComponent<ITraveler>();


    private void OnEnable()
    {
        NetInit?.TryActivate(this);
    }

    private void OnDisable()
    {
        NetInit?.TryDeactivate(this);
    }

    public void ActivateAt(INetAuthority authority)
    {
        if (!authority.IsOwner) { enabled = false; return; }
        TargetCamera?.MoveTo(this);
    }

    public void DeactivateAt(INetAuthority authority)
    {
        if (!authority.IsOwner) { return; }
        TargetCamera?.Stop();
    }
}
