using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "PlayerInputConfig", menuName = "PnR/Config/PlayerInput")]
public class PlayerInputConfig : ScriptableObject, IInputConfig
{
    [Header("Input Delay")]
    [field: SerializeField]
    public float DiagonalDelay { get; private set; } = 0.08f;

    [field: SerializeField]
    public float ReleaseDelay { get; private set; } = 0.05f;


    [Header("DeadZone")]
    [field: SerializeField]
    public float DeadZone { get; private set; } = 0.5f;
}