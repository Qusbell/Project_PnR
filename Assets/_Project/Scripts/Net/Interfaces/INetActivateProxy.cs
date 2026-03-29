
/// <summary>
/// Net상에서의 MonoBehaviour의 활성화/비활성화를 담당하는 인터페이스 <br/>
/// NetworkBehavior를 최소한으로 사용하기 위해, <br/>
/// MonoBehaviour의 활성화/비활성화 시점에 맞게 ActivateAt/DeactivateAt 메서드를 호출하는 역할을 담당 <br/>
/// </summary>
public interface INetActivateProxy
{
    /// <summary>
    /// ActivatedAt을 시도, 불가능하다면 ActivateAt을 예약
    /// </summary>
    void TryActivate(INetAware self);

    void TryDeactivate(INetAware self);
}
