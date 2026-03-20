using UnityEngine;
using System;

/// <summary>
/// 플레이어의 입력을 추상화하며 이동 및 충전 공격 상태를 제공하는 인터페이스
/// </summary>
public interface IPlayerInput
{
    Vector2 MoveInput { get; }
    bool IsCharging { get; }
    float ChargeRatio { get; } // 0.0 ~ 1.0

    // 공격 발생 시 호출 (충전량 전달)
    event Action<float> OnAttackReleased;

    void Initialize();
    void DisableInput();
}