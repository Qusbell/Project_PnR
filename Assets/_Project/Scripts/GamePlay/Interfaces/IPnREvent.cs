using System;
using UnityEngine;

/// <summary>
/// Press & Release Event : 플레이어의 눌렀을 떼, 뗄 때 행동
/// </summary>
public interface IPnREvent
{
    public event Action<Vector2> OnPressStarted;
    public event Action<Vector2> OnPressConfirmed;
    public event Action<Vector2> OnReleaseStarted;
    public event Action<Vector2> OnReleaseConfirmed;
}
