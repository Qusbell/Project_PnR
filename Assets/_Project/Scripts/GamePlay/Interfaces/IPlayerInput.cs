using UnityEngine;

/// <summary>
/// 플레이어의 입력을 추상화하는 인터페이스
/// </summary>
public interface IPlayerInput
{
    Vector2 MoveInput { get; }
    void Initialize();
    void DisableInput();
}