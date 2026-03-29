using UnityEngine;

[CreateAssetMenu(fileName = "NewFactionConfig", menuName = "PnR/Config/Entity/Faction")]
public class FactionConfig : ScriptableObject
{
    [field: SerializeField]
    public EFaction Faction { get; private set; }
}