
public interface INetAuthority
{
    bool IsOwner { get; }

    bool IsServer { get; }

    bool IsClient { get; }
}
