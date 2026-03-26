using System;
using UnityEngine;

/// <summary>
/// 목적지로 이동하는 오브젝트를 표현 <br/>
/// 목적지를 향해 길을 찾는 여행자
/// </summary>
public interface ITraveler
{
    /// <summary>
    /// 목적지 지정
    /// </summary>
    /// <param name="destination"></param>
    /// <returns>true : 갈 수 있음, false : 갈 수 없음</returns>
    bool MoveTo(IDestination destination);

    void Stop();
}
