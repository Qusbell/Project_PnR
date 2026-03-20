using UnityEngine;

/// <summary>
/// 캐릭터 또는 오브젝트의 이동을 담당하는 인터페이스
/// </summary>
public interface IMover
{
    float MoveSpeed { get; }
    void Move(Vector2 direction);
}