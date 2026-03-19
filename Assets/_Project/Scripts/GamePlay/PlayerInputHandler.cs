using UnityEngine;

public class PlayerInputHandler : MonoBehaviour, IPlayerInput
{
    private InputSystem_Actions _inputActions;
    private Vector2 _moveInput;

    private InputSystem_Actions InputActions => _inputActions ??= new InputSystem_Actions();
    public Vector2 MoveInput => _moveInput;

    public void Initialize()
    {
        InputActions.Player.Enable();
    }

    public void DisableInput()
    {
        _inputActions?.Player.Disable();
    }

    private void Update()
    {
        _moveInput = InputActions.Player.Move.ReadValue<Vector2>();
    }
}