using System;

/// <summary>
/// RawInput이 발생했을 때, 그 입력이 눌렸는지, 떼졌는지를 전달 <br/>
/// 방향성 입력에 한정
/// </summary>
public interface IRawDirectionalInput : ICompass
{
    event Action OnPressed;
    event Action OnReleased;
}
