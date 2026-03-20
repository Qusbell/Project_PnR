using System;
using UnityEngine;

/// <summary>
/// Press & Release Event : ÇÃ·¹ÀÌ¾îÀÇ ´­·¶À» ¶¼, ¶¿ ¶§ Çàµ¿
/// </summary>
public interface IPnREvents
{
    public event Action<float> OnPressed;
    public event Action<float, Vector2> OnReleased;
}
