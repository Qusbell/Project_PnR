using UnityEngine;

public class BaseFlag : IFlag
{
    public bool IsBlocked { get; private set; }

    public bool TryEnter()
    {
        if (IsBlocked) { return false; }
        return IsBlocked = true;
    }

    public void Enter()
    {
        IsBlocked = true;
    }

    public void Exit()
    {
        IsBlocked = false;
    }
}
