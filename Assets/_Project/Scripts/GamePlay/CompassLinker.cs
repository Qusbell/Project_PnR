using UnityEngine;


/// <summary>
/// 나침반을 다른 기능들과 연결시켜줌
/// </summary>
[RequireComponent (typeof(IDriver))]
[RequireComponent(typeof(ICompass))]
public class CompassLinker : MonoBehaviour
{
    // === Component === //

    private IDriver _driver;
    private IDriver Driver => _driver ??= GetComponent<IDriver>();

    private ITargeter _targeter;
    private ITargeter Targeter => _targeter ??= GetComponent<ITargeter>();


    // === Life Cycle === //

    private void OnEnable()
    {
        if (TryGetComponent<IRawDirectionalInput>(out var RawCompass))       { Driver?.MoveBy(RawCompass); }
        if (TryGetComponent<IIntentDirectionalInput>(out var IntentCompass)) { Targeter?.LookBy(IntentCompass); }
    }

}
