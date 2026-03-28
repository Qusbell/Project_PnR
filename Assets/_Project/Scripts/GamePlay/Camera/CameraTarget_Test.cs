using UnityEngine;

public class CameraTarget_Test : NetAwareBehavior, IDestination, INetAware
{
    public Vector2 Position => transform.position;

    public bool IsActivated => enabled;
        
    private ITraveler _targetCamera;
    private ITraveler TargetCamera => _targetCamera ??= Camera.main?.GetComponent<ITraveler>();


    public override void ActivateAt(INetAuthority authority)
    {
        if (!authority.IsOwner) { enabled = false; return; }
        TargetCamera?.MoveTo(this);
    }

    public override void DeactivateAt(INetAuthority authority)
    {
        if (!authority.IsOwner) { return; }
        TargetCamera?.Stop();
    }
}
