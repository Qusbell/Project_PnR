using UnityEngine;


/// <summary>
/// IPnREvent에 연결하여, Press와 Release 시의 처리를 실행
/// </summary>
[RequireComponent (typeof(IPnREvent))]
public abstract class PnRAction : MonoBehaviour
{
    // ==== Component ====

    private IPnREvent _pnrEvents;
    private IPnREvent PnREvent => _pnrEvents ??= GetComponent<IPnREvent>();


    // ==== Life Cycle ====

    protected virtual void OnEnable()
    {
        PnREvent.OnPressed += OnPressed;
        PnREvent.OnReleased += OnReleased;
    }

    protected virtual void OnDisable()
    {
        PnREvent.OnPressed -= OnPressed;
        PnREvent.OnReleased -= OnReleased;
    }


    // ==== Custom ====

    protected virtual void OnPressed(float time) { }

    protected virtual void OnReleased(float time, Vector2 direction) { }

}
