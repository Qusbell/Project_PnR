using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityTypeConfig", menuName = "PnR/Config/Entity/EntityType")]
public class EntityTypeConfig : ScriptableObject
{
    [field: SerializeField]
    public EEntityType Type { get; private set; }
}
