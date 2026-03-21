using UnityEngine;


/// <summary>
/// IMovable과 ICompass를 이어줌
/// </summary>
[RequireComponent (typeof(IMovable))]
[RequireComponent(typeof(ICompass))]
public class MovementLinker : MonoBehaviour
{
    // ==== Component ====

    private IMovable _movable;
    private IMovable Movable => _movable ??= GetComponent<IMovable>();

    private ICompass _compass;
    private ICompass Compass => _compass ??= GetComponent<ICompass>();


    // ==== Life Cycle ====

    private void Update()
    {
        Movable.Move(Compass.Direction);
    }
}
