using UnityEngine;

/// <summary>
/// 최종적으로 향하게 될 목적지
/// </summary>
public interface IDestination
{
    Vector2 Position { get; }

    bool IsActivated { get; }
}
