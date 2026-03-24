using UnityEngine;

public interface INetActivator
{
    /// <summary>
    /// ActivatedAt을 시도, 불가능하다면 ActivateAt을 예약
    /// </summary>
    /// <param name="target"></param>
    void TryActivate(INetAware target);

    void TryDeactivate(INetAware target);
}
