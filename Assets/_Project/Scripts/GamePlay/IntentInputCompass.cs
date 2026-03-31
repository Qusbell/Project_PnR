using System;
using UnityEngine;


/// <summary>
/// 실제 의도를 반영한 입력값을 반환하는 나침반
/// </summary>
public class IntentInputCompass : MonoBehaviour, IIntentDirectionalInput
{
    private IRawDirectionalInput _rawInput;
    private IRawDirectionalInput RawInput => _rawInput ??= GetComponent<IRawDirectionalInput>();



    public Vector2 Direction => throw new NotImplementedException();

    public bool IsActivate => throw new NotImplementedException();



    public event Action<Vector2> OnPressStarted;
    public event Action<Vector2> OnPressConfirmed;
    public event Action<Vector2> OnReleaseStarted;
    public event Action<Vector2> OnReleaseConfirmed;




}
