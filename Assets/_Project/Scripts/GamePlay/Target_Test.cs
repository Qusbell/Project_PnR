using UnityEngine;

public class Target_Test : MonoBehaviour, ITargetable
{
    public IEntityContext Context { get; }

    public Vector3 Position => transform.position;

    public bool IsTargetable => true;
}
