// using System;
// using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Entity의 각종 특징을 담는 Context <br/>
/// 딕셔너리 캐싱 방식 고민 중 (과연 Feature를 그렇게까지 자주 조회해야 할까?)
/// </summary>
public class EntityContext : NetAwareBehavior, IEntityContext
{
    // === Field === //

    [SerializeField] private INetID _id;
    [SerializeField] private EntityTypeConfig _type;
    [SerializeField] private FactionConfig _faction;

    public ulong ID => _id != null ? _id.NetworkObjectId : 0;

    public EEntityType EntityType => _type != null ? _type.Type : EEntityType.Object;

    public EFaction Faction => _faction != null ? _faction.Faction : EFaction.Neutral;


    // === Interface === //

    public override void ActivateAt(INetContext authority)
    {
        _id = authority;
        // 싱글톤 등록
    }
 
    public override void DeactivateAt(INetContext authority)
    {

    }

    public bool TryGetFeature<T>(out T feature) where T : IEntityFeature
    {
        // T = 인터페이스인지 검사
        if (!typeof(T).IsInterface)
        {
            feature = default;
            return false;
        }
        
        return TryGetComponent(out feature);
    }



    // === Debug === //

    private void OnValidate()
    {
        if (_type == null)
        {
            Debug.LogError($"{gameObject.name} : EntityContext : Type 미할당");
        }

        if (_faction == null)
        {
            Debug.LogError($"{gameObject.name} : EntityContext : Faction 미할당");
        }
    }
}