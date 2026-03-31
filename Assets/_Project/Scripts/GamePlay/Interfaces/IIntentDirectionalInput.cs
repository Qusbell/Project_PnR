using System;
using UnityEngine;

/// <summary>
/// RawInput의 의도를 파악하여, IntentInput이 된 값을 전달 <br/>
/// 방향성 입력에 한정
/// </summary>
public interface IIntentDirectionalInput : ICompass
{
    public event Action<Vector2> OnPressStarted;
    public event Action<Vector2> OnPressConfirmed;
    public event Action<Vector2> OnReleaseStarted;
    public event Action<Vector2> OnReleaseConfirmed;
}
