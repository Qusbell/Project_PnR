using System;
using UnityEngine;


/// <summary>
/// IPnREvent에 연결하여, Press와 Release 시의 처리를 실행 <br/>
/// <-- 이거 NetAware 필요할듯?
/// </summary>
[RequireComponent (typeof(IIntentDirectionalInput))]
public abstract class PnRAction : MonoBehaviour
{
    // ==== Component ====

    private IIntentDirectionalInput _pnrEvents;
    private IIntentDirectionalInput PnREvent => _pnrEvents ??= GetComponent<IIntentDirectionalInput>();


    // ==== Life Cycle ====

    protected virtual void OnEnable()
    {
        PnREvent.OnPressStarted += OnPressStarted;
        PnREvent.OnPressConfirmed += OnPressConfirmed;
        PnREvent.OnReleaseStarted += OnReleasStarted;
        PnREvent.OnReleaseConfirmed += OnReleaseConfirmed;
    }

    protected virtual void OnDisable()
    {
        PnREvent.OnPressStarted -= OnPressStarted;
        PnREvent.OnPressConfirmed -= OnPressConfirmed;
        PnREvent.OnReleaseStarted -= OnReleasStarted;
        PnREvent.OnReleaseConfirmed -= OnReleaseConfirmed;
    }

    // ==== Custom ====

    protected virtual void OnPressStarted(Vector2 direction) { }

    protected virtual void OnPressConfirmed(Vector2 direction) { }

    protected virtual void OnReleasStarted(Vector2 direction) { }

    protected virtual void OnReleaseConfirmed(Vector2 direction) { }

}
