using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private InputSystem_Actions _inputActions;
    private InputSystem_Actions InputActions => _inputActions ??= new @InputSystem_Actions();

    public Vector2 MoveInput { get; private set; }

    private Vector2 IntentInput { get; set; }
    // <-- 순환큐로 대각선 방향+시간 intent 저장

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
        MoveInput = InputActions.Player.Move.ReadValue<Vector2>();

        // IntentInput(대각선 입력 등) 처리
        if (MoveInput != Vector2.zero)
        {
            IntentInput = MoveInput; // 혹은 순환 큐에 데이터 삽입 로직
        }
    }

}
