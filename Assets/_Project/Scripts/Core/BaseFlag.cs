using UnityEngine;

public class BaseFlag : IFlag
{
    public bool IsFlagOn { get; private set; }

    public bool TryEnter()
    {
        if (IsFlagOn) { return false; }
        return IsFlagOn = true;
    }

    public void Exit()
    {
        IsFlagOn = false;
    }
}
