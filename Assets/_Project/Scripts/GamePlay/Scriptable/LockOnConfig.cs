using UnityEngine;

[CreateAssetMenu(fileName = "NewLockOnConfig", menuName = "PnR/Config/Action/LockOnConfig")]
public class LockOnConfig : ScriptableObject
{
    [field: SerializeField]
    public float Distance { get; private set; } = 10f;

    [field: SerializeField]
    public float Angle { get; private set; } = 45f;

    [field: SerializeField]
    public LayerMask TargetLayer { get; private set; }
}
