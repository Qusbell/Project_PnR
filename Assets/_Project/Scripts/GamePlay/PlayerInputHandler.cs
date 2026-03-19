using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private InputSystem_Actions _inputActions;
    public Vector2 MoveInput { get; private set; }

    public void Initialize()
    {
        _inputActions = new InputSystem_Actions();
        _inputActions.Player.Enable();
    }

    public void DisableInput()
    {
        _inputActions?.Player.Disable();
    }

    void Update()
    {
        MoveInput = _inputActions.Player.Move.ReadValue<Vector2>();
    }
}