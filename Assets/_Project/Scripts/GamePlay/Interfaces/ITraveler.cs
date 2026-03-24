using System;
using UnityEngine;

/// <summary>
/// 목적지로 이동하는 오브젝트를 표현 <br/>
/// 목적지를 향해 길을 찾는 여행자
/// </summary>
public interface ITraveler
{
    void MoveTo(IDestination destination);

    void Stop();
}
