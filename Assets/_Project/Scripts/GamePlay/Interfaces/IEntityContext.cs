
/// <summary>
/// Entity의 Context를 나타내는 인터페이스
/// </summary>
public interface IEntityContext
{
    /// <summary>
    /// Net상에서 Entity를 식별하기 위한 고유 ID <br/>
    /// NetworkObjectID와 동일
    /// </summary>
    ulong ID { get; }

    EEntityType EntityType { get; }
    EFaction Faction { get; }

    /// <summary>
    /// Entity가 가지고 있는 특징을 가져오는 메서드
    /// </summary>
    /// <typeparam name="T">찾을 특징</typeparam>
    /// <param name="feature">찾아낸 특징</param>
    /// <returns>탐색 성공 여부</returns>
    bool TryGetFeature<T>(out T feature) where T : IEntityFeature;
}
