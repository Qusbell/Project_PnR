using UnityEngine;

public class CameraTarget_Test : NetMember_Test, IDestination, INetAware
{
    public Vector2 Position => transform.position;

    public bool IsActivated => enabled;

    private ITraveler targetCamera;
    private ITraveler TargetCamera => targetCamera ??= Camera.main?.GetComponent<ITraveler>();


    protected override void OnDisable()
    {
        base.OnDisable();
    }

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
