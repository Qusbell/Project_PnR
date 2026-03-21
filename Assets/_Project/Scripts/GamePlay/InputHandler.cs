using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour, IPnREvents, ICompass
{
    private InputSystem_Actions _inputActions;
    private InputSystem_Actions InputActions => _inputActions ??= new();
    
    /// <summary>
    /// 인스펙터 할당 필요
    /// </summary>
    [field:SerializeField]
    private InputConfig InputConfig { get; set; }


    // 입력 방향
    public Vector2 Direction { get; private set; }

    // 의도 방향
    private Vector2 IntentInput
    {
        get
        {
            var direction = Intents?.GetIntent();
            if (direction.HasValue) { return direction.Value; }
            else                    { return Direction; }
        }
    }

    // 대각선 의도
    private IntentBuffer _intentBuffer;
    private IntentBuffer Intents => _intentBuffer ??= new(InputConfig.DiagonalDelay, 20, InputConfig.DeadZone);

    // 시간 + 방향 전달
    public event Action<float> OnPressed;
    public event Action<float, Vector2> OnReleased;


    private void StartPressed(InputAction.CallbackContext context)
    {
        OnPressed?.Invoke(Time.time);
    }

    private void EndReleased(InputAction.CallbackContext context)
    {
        OnReleased?.Invoke(Time.time, IntentInput);
    }


    private void OnEnable()
    {
        InputActions.Player.Enable();
        InputActions.Player.Move.started += StartPressed;
        InputActions.Player.Move.canceled += EndReleased;
    }

    private void OnDisable()
    {
        InputActions.Player.Move.started -= StartPressed;
        InputActions.Player.Move.canceled -= EndReleased;
        InputActions.Player.Disable();
    }


    private void Update()
    {
        // 매 프레임 현재 입력 값을 읽어오기
        Direction = InputActions.Player.Move.ReadValue<Vector2>();

        // IntentInput(대각선 입력 등) 처리
        if (Direction != Vector2.zero)
        {
            Intents?.SetIntent(Direction, Time.time);
        }
    }

}
