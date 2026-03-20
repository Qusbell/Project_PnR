using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInputConfig", menuName = "PnR/Config/PlayerInput")]
public class PlayerInputConfig : ScriptableObject
{
    [field: SerializeField, Header("Charging Settings")]
    public float MaxChargeTime { get; private set; } = 2.0f;

    [field: SerializeField]
    public float MinChargeThreshold { get; private set; } = 0.2f;

    [field: SerializeField, Header("Attack Settings")]
    public float BaseDamage { get; private set; } = 10f;

    [field: SerializeField]
    public float MaxDamageMultiplier { get; private set; } = 3.0f;
}