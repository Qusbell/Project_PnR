using UnityEngine;


[CreateAssetMenu(fileName = "CameraConfig", menuName = "PnR/Config/Camera")]
public class CameraConfig : ScriptableObject
{
    [field: SerializeField]
    public float SmoothTime { get; private set; } = 0.15f;


    [field: SerializeField]
    public Vector3 Offset { get; private set; } = new Vector3(0, 0, -10);


    [field: SerializeField]
    public float MaxSpeed { get; private set; } = 30f;
}
