using UnityEngine;

/// <summary>
/// Not-Network class에 할당 <br/>
/// 현재 이 클래스가  어디에 있는지 검사하고 그에 알맞는 동작 실행
/// </summary>
public interface INetAware
{
    void Initialize(INetAuthority authority);
}
