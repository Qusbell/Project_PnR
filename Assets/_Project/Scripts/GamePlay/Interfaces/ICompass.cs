using UnityEngine;

/// <summary>
/// 특정 방향으로의 지향성을 표현, 방향 제공 <br/>
/// 어딘가로 방향을 가리키는 나침반
/// </summary>
public interface ICompass
{
    Vector2 Direction { get; }

    bool IsActivate { get; }
}
