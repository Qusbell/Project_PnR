using System;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

/// <summary>
/// 입력값을 제어하고 <br/>
/// 플레이어의 의도대로 움직일 수 있도록 보조 <br/>
/// 의도 보조 예시: <br/>
/// - 대각선 Release 일관성 형성 <br/>
/// - ← → 동시 입력 중 Release 되는 현상 제거 <br/>
/// - ←입력 중 →입력 시 아주 잠시 Release되는 현상 제거
/// </summary>
public class InputHandler : MonoBehaviour, IPnREvent, ICompass, INetAware
{
    // ==== Field ====

    private InputSystem_Actions _inputActions;
    private InputSystem_Actions InputActions => _inputActions ??= new();
    
    /// <summary>
    /// 인스펙터 할당 필수
    /// </summary>
    [field:SerializeField]
    private InputConfig InputConfig { get; set; }

    // 시간 + 방향 전달
    public event Action<Vector2> OnPressStarted;
    public event Action<Vector2> OnPressConfirmed;
    public event Action<Vector2> OnReleaseStarted;
    public event Action<Vector2> OnReleaseConfirmed;

    // 입력 방향
    public Vector2 Direction { get; private set; }



    // ---- 대각선 의도 ----

    /// <summary>
    /// 대각선 의도 <br/>
    /// new(대각선 의도 판정 시간, 의도 갯수, 대각선 최소 크기)
    /// </summary>
    private IntentBuffer _intentBuffer;
    private IntentBuffer Intents => _intentBuffer ??= new(InputConfig.DiagonalDelay, InputConfig.DeadZone);

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


    // ---- 동시 입력 Release 억제 ----

    /// <summary>
    /// 이미 누르고 있는 상태인지 체크 Flag <br/>
    /// started 과다 발생 억제 <br/>
    /// <-- alt + tab 사용 등으로 창을 나갈 경우, canceled 이벤트가 들어오지 않아 굳을 수 있음 <br/>
    /// 이 예외처리가 당장 필요하진 않겠지만, 이에 대해서 인지해둘 것
    /// </summary>
    private bool IsPressStarted { get; set; } = false;

    /// <summary>
    /// IsPhysicallyPressed에서 사용할 버튼들 전수조사 저장용 List
    /// </summary>
    private List<ButtonControl> _moveButtons;
    private List<ButtonControl> MoveButtons => _moveButtons ??= new();

    /// <summary>
    /// 키보드에서 ← + → 입력 시 Released 나가는 거 때문에(Vector2.zero를 무조건 canceled로 판정해버림)<br/>
    /// 하다하다 안 돼서 그냥 물리적인 키 입력 자체를 전수조사하는 방법을 채택
    /// </summary>
    private bool IsPhysicallyPressed
    {
        get
        {
            // MoveAt 액션에 바인딩된 모든 컨트롤(키) 중 하나라도 눌려있는지 확인
            foreach (var button in MoveButtons)
            {
                if (button.isPressed) { return true; }
            }
            return false;
        }
    }

    // ---- 실수 Release 억제 ----

    /// <summary>
    /// 매 Pressed마다 ++
    /// 실제 Release를 실행할 때 ID가 다르면 Cancel
    /// </summary>
    private int ReleaseConfirmID { get; set; } = 0;

    /// <summary>
    /// Release가 Confirm 받고 있는 동안 true
    /// </summary>
    private bool IsReleasePending { get; set; } = false;


    // ==== Life Cycle ====

    // <-- Initialize랑 순서가 잘못될 수 있음
    // 이후 Initialize에서 초기화 검토
    // (Disable도 마찬가지)
    private void OnEnable()
    {
        InputActions.Player.Enable();
        InputActions.Player.Move.started += StartPressed;
        InputActions.Player.Move.canceled += EndReleased;

        // 버튼 초기화
        foreach (var control in InputActions.Player.Move.controls)
        {
            if (control is UnityEngine.InputSystem.Controls.ButtonControl button)
            {
                MoveButtons.Add(button);
            }
        }
    }


    private void OnDisable()
    {
        InputActions.Player.Move.started -= StartPressed;
        InputActions.Player.Move.canceled -= EndReleased;
        InputActions.Player.Disable();

        MoveButtons.Clear();
    }


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


    // ==== Custom ====

    public void Initialize(INetAuthority authority)
    {
        if (!authority.IsOwner)
        {
            enabled = false;
        }
    }

    private void StartPressed(InputAction.CallbackContext context)
    {
        if(IsPressStarted) { return; }
        IsPressStarted = true;

        var direction = InputActions.Player.Move.ReadValue<Vector2>();

        ReleaseConfirmID++;

        if(!IsReleasePending)
        {
            OnPressStarted?.Invoke(direction);
            _ = ConfirmPress();
        }
    }

    private async Awaitable ConfirmPress()
    {
        await Awaitable.WaitForSecondsAsync(InputConfig.InputDelay, destroyCancellationToken);

        if (this.enabled)
        {
            // Press가 결정된 타이밍의 Intent 전달
            OnPressConfirmed?.Invoke(IntentInput);
        }
    }

    private void EndReleased(InputAction.CallbackContext context)
    {
        if(IsPhysicallyPressed) { return; } // <-- 이거 키보드에서만 먹힐 거 같은데? 나중에 체크해봐야겠다
        IsPressStarted = false;

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
        // Confirm 시작
        IsReleasePending = true;

        // 2. Delay 실행
        await Awaitable.WaitForSecondsAsync(InputConfig.InputDelay, destroyCancellationToken);

        // 3. Delay 이후에도 Press가 없었다면,
        // 그제서야 Release로 간주
        if (ID == ReleaseConfirmID && this.enabled)
        {
            // <-- Release 직후 esc 등을 눌러서 pause 상태가 된 경우,
            // 잘못된 Release가 발생할 수 있을 것 같음
            // 그런 상황이 없다면 다행이지만 있다면 이 부분 확인
            OnReleaseConfirmed?.Invoke(direction);
        }

        // 어쨌든 Confirm은 끝났음
        IsReleasePending = false;
    }



    // ==== Debug ====

    private void OnValidate()
    {
        if (InputConfig == null)
        {
            Debug.LogError($"{name} : InputConfig = null");
        }
    }

}
