using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 물리적인 키 입력값을 그대로 반환하는 나침반 <br/>
/// Network상에서의 초기화 필요
/// </summary>
public class RawInputCompass : NetAwareBehavior, IRawDirectionalInput
{
    // === Field === //

    private InputSystem_Actions InputActions { get; set; }

    // 입력 방향
    public Vector2 Direction => InputActions?.Player.Move.ReadValue<Vector2>() ?? Vector2.zero;

    public bool IsActivate => enabled && Direction.sqrMagnitude > 0.01f;

    public event Action OnPressed;
    public event Action OnReleased;


    // --- 동시 입력 Release 억제 --- //

    /// <summary>
    /// 이미 누르고 있는 상태인지 체크 Flag <br/>
    /// started 과다 발생 억제 <br/>
    /// <-- alt + tab 사용 등으로 창을 나갈 경우, canceled 이벤트가 들어오지 않아 굳을 수 있음 <br/>
    /// 이 예외처리가 당장 필요하진 않겠지만, 이에 대해서 인지해둘 것
    /// </summary>
    private IFlag PressingFlag { get; set; } = new BaseFlag();

    /// <summary>
    /// 실제 물리적 버튼 입력을 체크
    /// </summary>
    private ButtonInputValidator ButtonInput { get; set; }


    // === Method === //

    public override void ActivateAt(INetContext authority)
    {
        if (!authority.IsOwner) { return; }

        InputActions ??= new();
        ButtonInput ??= new(InputActions.Player.Move.controls);

        InputActions.Player.Move.started -= Pressed;
        InputActions.Player.Move.started += Pressed;

        InputActions.Player.Move.canceled -= Released;
        InputActions.Player.Move.canceled += Released;
    }

    public override void DeactivateAt(INetContext authority)
    {
        if (!authority.IsOwner) { return; }

        InputActions.Player.Move.started -= Pressed;
        InputActions.Player.Move.canceled -= Released;
    }


    private void Pressed(InputAction.CallbackContext context)
    {
        if (!PressingFlag.TryEnter()) { return; }

        OnPressed?.Invoke();
    }

    private void Released(InputAction.CallbackContext context)
    {
        if (ButtonInput.IsPhysicallyPressed) { return; }
        PressingFlag.Exit();

        OnReleased?.Invoke();
    }


    private void OnDestroy()
    {
        InputActions?.Dispose();
    }

}
