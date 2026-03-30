using System;

public interface ITargeter
{
    ITargetable Target { get; }

    void LookBy(ICompass compass);

    event Action<ITargetable> OnTargetChanged;
}
