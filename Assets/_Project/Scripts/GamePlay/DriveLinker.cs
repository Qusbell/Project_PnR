using UnityEngine;


/// <summary>
/// 방향성 동작을 이어줌
/// </summary>
[RequireComponent (typeof(IDriver))]
[RequireComponent(typeof(ICompass))]
public class DriveLinker : MonoBehaviour
{
    // ==== Component ====

    private IDriver _driver;
    private IDriver Driver => _driver ??= GetComponent<IDriver>();

    private ITargeter _targeter;
    private ITargeter Targeter => _targeter ??= GetComponent<ITargeter>();

    private ICompass _compass;
    private ICompass Compass => _compass ??= GetComponent<ICompass>();


    // ==== Life Cycle ====

    private void Update()
    {
        Driver.MoveAt(Compass);
        Targeter.LookAt(Compass);
    }
}
