using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using System.Collections.Generic;

/// <summary>
/// 입력값을 제어하고 <br/>
/// 플레이어의 의도대로 움직일 수 있도록 보조 <br/>
/// 의도 보조 예시: <br/>
/// - 대각선 Release 일관성 형성 <br/>
/// - ← → 동시 입력 중 Release 되는 현상 제거 <br/>
/// - ←입력 중 →입력 시 아주 잠시 Release되는 현상 제거
/// </summary>
public class InputHandler : NetworkBehaviour, IPnREvent, ICompass
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
    public event Action<float> OnPressed;
    public event Action<float, Vector2> OnReleased;

    // 입력 방향
    public Vector2 Direction { get; private set; }



    // ---- 대각선 의도 ----

    // 대각선 의도
    // new(대각선 의도 판정 시간, 의도 갯수, 대각선 최소 크기)
    // 여기서 의도 갯수는 프레임따라 달라질 수 있음 주의
    private IntentBuffer _intentBuffer;
    private IntentBuffer Intents => _intentBuffer ??= new(InputConfig.DiagonalDelay, 20, InputConfig.DeadZone);

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
    /// <-- Warning: alt + tab 사용 등으로 창을 나갈 경우, canceled 이벤트가 들어오지 않아 굳을 수 있음 <br/>
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
            // Move 액션에 바인딩된 모든 컨트롤(키) 중 하나라도 눌려있는지 확인
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


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner)
        {
            enabled = false;
        }
    }

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
            Intents?.SetIntent(Direction, Time.time);
        }
    }



    // ==== Custom ====

    private void StartPressed(InputAction.CallbackContext context)
    {
        if(IsPressStarted) { return; }
        IsPressStarted = true;

        ReleaseConfirmID++;

        if(!IsReleasePending)
        {
            OnPressed?.Invoke(Time.time);
        }
    }

    private void EndReleased(InputAction.CallbackContext context)
    {
        if(IsPhysicallyPressed) { return; } // <-- 이거 키보드에서만 먹힐 거 같은데? 나중에 체크해봐야겠다
        IsPressStarted = false;

        _ = ConfirmRelease(ReleaseConfirmID);
    }


    /// <summary>
    /// Release를 실제로 발생시킬지 검토
    /// </summary>
    private async Awaitable ConfirmRelease(int ID)
    {
        // Confirm 시작
        IsReleasePending = true;

        try
        {
            // 1. Delay 이전에 Intent 저장 (시간 경과로 인한 Intent 휘발 방지)
            var direction = IntentInput;

            // 2. Delay 실행
            await Awaitable.WaitForSecondsAsync(InputConfig.ReleaseDelay, destroyCancellationToken);

            // 3. Delay 이후에도 Press가 없었다면,
            // 그제서야 Release로 간주
            if (ID == ReleaseConfirmID && this.enabled)
            {
                OnReleased?.Invoke(Time.time, direction);
            }
        }
        catch (OperationCanceledException)
        {
            // 오브젝트 파괴 시 정리
        }
        finally
        {
            // 어쨌든 Confirm은 끝났음
            IsReleasePending = false;
        }
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
