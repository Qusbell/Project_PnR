using UnityEngine;

/// <summary>
/// 특정 방향으로의 지향성을 표현, 방향 제공
/// </summary>
public interface ICompass
{
    Vector2 Direction { get; }
}
