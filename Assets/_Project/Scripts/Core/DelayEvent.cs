using UnityEngine;

public class DelayEvent
{
    private int _currentID = 0;

    public void Invoke(float delay, System.Action callback)
    {
        InvokeAsync(++_currentID, delay, callback);
    }

    public void Cancel()
    {
        _currentID++;
    }

    private async void InvokeAsync(int id, float delay, System.Action callback)
    {
        await System.Threading.Tasks.Task.Delay((int)(delay * 1000));

        if (id == _currentID)
        { callback?.Invoke(); }
    }

}
