using UnityEngine;

/// <summary>
/// Unity Input System을 사용하여 플레이어 입력을 처리합니다.
/// </summary>
public class PlayerInputHandler : MonoBehaviour, IPlayerInput
{
    private InputSystem_Actions _inputActions;
    private InputSystem_Actions InputActions => _inputActions ??= new InputSystem_Actions();

    [field: SerializeField]
    public Vector2 MoveInput { get; private set; }

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
        // 프로퍼티를 통해서만 값을 설정합니다.
        MoveInput = InputActions.Player.Move.ReadValue<Vector2>();
    }
}