using UnityEngine;

/// <summary>
/// 이동 가능한 오브젝트를 표현
/// </summary>
public interface IMovable
{
    float MoveSpeed { get; }
    void Move(Vector2 direction);
}