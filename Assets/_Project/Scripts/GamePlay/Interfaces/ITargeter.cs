using System;

public interface ITargeter
{
    ITargetable Target { get; }

    void LookAt(ICompass compass);

    event Action<ITargetable> OnTargetChanged;
}
