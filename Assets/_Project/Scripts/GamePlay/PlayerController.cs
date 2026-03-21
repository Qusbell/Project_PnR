using UnityEngine;


[RequireComponent (typeof(IMovable))]
[RequireComponent (typeof(ICompass))]
public class PlayerController : MonoBehaviour
{
    private IMovable _movable;
    private IMovable Movable => _movable ??= GetComponent<IMovable>();


    private ICompass _compass;
    private ICompass Compass => _compass ??= GetComponent<ICompass>();


    private void Update()
    {
        Movable.Move(Compass.Direction);
    }

}
