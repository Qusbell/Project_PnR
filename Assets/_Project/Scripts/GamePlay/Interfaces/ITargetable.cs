using System.Numerics;

/// <summary>
/// 대상으로 지정될 수 있는 개체에 대한 인터페이스
/// </summary>
public interface ITargetable
{
    IEntityContext Context { get; }

    Vector2 Position { get; }

    bool IsTargetable { get; }
}
