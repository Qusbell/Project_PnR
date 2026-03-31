using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 입력값을 제어하고 <br/>
/// 플레이어의 의도대로 움직일 수 있도록 보조 <br/>
/// 의도 보조 예시: <br/>
/// - 대각선 Release 일관성 형성 <br/>
/// - ← → 동시 입력 중 Release 되는 현상 제거 <br/>
/// - ←입력 중 →입력 시 아주 잠시 Release되는 현상 제거
/// </summary>
public class InputHandler : NetAwareBehavior, IPnREvent, ICompass
{
    // === Field === //

    private InputSystem_Actions _inputActions;
    private InputSystem_Actions InputActions => _inputActions ??= new();


    /// <summary>
    /// 인스펙터 할당 필수
    /// </summary>
    [field: SerializeField]
    private InputConfig Config { get; set; }

    // 시간 + 방향 전달
    public event Action<Vector2> OnPressStarted;
    public event Action<Vector2> OnPressConfirmed;
    public event Action<Vector2> OnReleaseStarted;
    public event Action<Vector2> OnReleaseConfirmed;

    // 입력 방향
    public Vector2 Direction { get; private set; }

    public bool IsActivate => Direction.sqrMagnitude > 0.01f;



    // --- 대각선 의도 --- //

    /// <summary>
    /// 대각선 의도 <br/>
    /// new(대각선 의도 판정 시간, 의도 갯수, 대각선 최소 크기)
    /// </summary>
    private IntentBuffer _intentBuffer;
    private IntentBuffer Intents => _intentBuffer ??= new(Config.DiagonalDelay, Config.DeadZone);

    /// <summary>
    /// 실제 입력값이 아닌, 플레이어의 의도가 반영된 입력 방향
    /// </summary>
    private Vector2 IntentInput
    {
        get
        {
            var direction = Intents?.GetIntent();
            return direction.HasValue ? direction.Value : Direction;
        }
    }


    // --- 동시 입력 Release 억제 --- //

    /// <summary>
    /// 이미 누르고 있는 상태인지 체크 Flag <br/>
    /// started 과다 발생 억제 <br/>
    /// <-- alt + tab 사용 등으로 창을 나갈 경우, canceled 이벤트가 들어오지 않아 굳을 수 있음 <br/>
    /// 이 예외처리가 당장 필요하진 않겠지만, 이에 대해서 인지해둘 것
    /// </summary>
    private IFlag PressingFlag { get; set; } = new BaseFlag();

    private ButtonInputValidator _buttonInput;
    private ButtonInputValidator ButtonInput => _buttonInput ??= new(InputActions.Player.Move.controls);


    // --- 실수에 의한 Release 억제 --- //

    /// <summary>
    /// 매 Pressed마다 ++
    /// 실제 Release를 실행할 때 ID가 다르면 Cancel
    /// </summary>
    private int ReleaseConfirmID { get; set; } = 0;

    /// <summary>
    /// Release Confirm 중인지 체크하는 Flag
    /// </summary>
    private IFlag ReleasingFlag { get; set; } = new BaseFlag();


    // === Life Cycle === //

    private void Update()
    {
        // 매 프레임 현재 입력 값을 읽어오기
        Direction = InputActions.Player.Move.ReadValue<Vector2>();

        // IntentInput(대각선 입력 등) 처리
        if (Direction != Vector2.zero)
        {
            Intents?.SetIntent(Direction, Time.unscaledTime);
        }
    }

    private void OnDestroy()
    {
        _inputActions?.Dispose();
    }


    // === Interface === //

    public override void ActivateAt(INetContext authority)
    {
        if (!authority.IsOwner) { return; }

        InputActions.Player.Enable();
        InputActions.Player.Move.started += StartPressed;
        InputActions.Player.Move.canceled += EndReleased;
    }

    public override void DeactivateAt(INetContext authority)
    {
        if (!authority.IsOwner) { return; }

        InputActions.Player.Move.started -= StartPressed;
        InputActions.Player.Move.canceled -= EndReleased;
        InputActions.Player.Disable();
    }


    // === Custom === //

    private void StartPressed(InputAction.CallbackContext context)
    {
        if (!PressingFlag.TryEnter()) { return; }

        var direction = InputActions.Player.Move.ReadValue<Vector2>();

        ReleaseConfirmID++;

        // Release Confirm 중이 아닐 때에만
        // Press Confirm 허용
        if(!ReleasingFlag.IsBlocked)
        {
            OnPressStarted?.Invoke(direction);
            _ = ConfirmPress();
        }
    }

    private async Awaitable ConfirmPress()
    {
        await Awaitable.WaitForSecondsAsync(Config.InputDelay, destroyCancellationToken);

        if (this.enabled)
        {
            // Press가 결정된 타이밍의 Intent 전달
            OnPressConfirmed?.Invoke(IntentInput);
        }
    }

    private void EndReleased(InputAction.CallbackContext context)
    {
        if (ButtonInput.IsPhysicallyPressed) { return; }
        PressingFlag.Exit();

        // 1. Delay 이전에 Intent 저장 (시간 경과로 인한 Intent 휘발 방지)
        var direction = IntentInput;
        // Confirm 이전의 Release
        OnReleaseStarted?.Invoke(direction);

        _ = ConfirmRelease(ReleaseConfirmID, direction);
    }

    /// <summary>
    /// Release를 실제로 발생시킬지 검토
    /// </summary>
    private async Awaitable ConfirmRelease(int ID, Vector2 direction)
    {
        ReleasingFlag.Enter();

        try
        {
            // 2. Delay 실행
            await Awaitable.WaitForSecondsAsync(Config.InputDelay, destroyCancellationToken);

            // 3. Delay 이후에도 Press가 없었다면,
            // 그제서야 Release로 간주
            if (ID == ReleaseConfirmID && this.enabled)
            {
                // <-- Release 직후 esc 등을 눌러서 pause 상태가 된 경우,
                // 잘못된 Release가 발생할 수 있을 것 같음
                // 그런 상황이 없다면 다행이지만 있다면 이 부분 확인
                OnReleaseConfirmed?.Invoke(direction);
            }
        }
        finally
        {
            // 비동기 작업이 취소되거나 오류가 나도 반드시 Flag를 해제함
            // 어쨌든 Confirm은 끝났음
            ReleasingFlag.Exit();
        }
    }



    // === Debug === //

    private void OnValidate()
    {
        if (Config == null)
        {
            Debug.LogError($"{name} : InputConfig = null");
        }
    }

}
