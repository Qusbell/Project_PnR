using UnityEngine;


/// <summary>
/// NetworkBehaviour에 할당 <br/>
/// 현재 자신의 네트워크 상의 권한 상태를 체크
/// </summary>
public interface INetAuthority
{
    bool IsOwner { get; }

    bool IsServer { get; }

    bool IsClient { get; }
}
