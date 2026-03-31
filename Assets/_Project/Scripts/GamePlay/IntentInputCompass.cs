using System;
using UnityEngine;


/// <summary>
/// 실제 의도를 반영한 입력값을 반환하는 나침반
/// </summary>
[RequireComponent(typeof(IRawDirectionalInput))]
public class IntentInputCompass : MonoBehaviour, IIntentDirectionalInput
{
    // === Field === //

    private IRawDirectionalInput RawInput { get; set; }

    public Vector2 RawDirection => RawInput?.Direction ?? Vector2.zero;


    /// <summary>
    /// 인스펙터 할당 필수
    /// </summary>
    [field: SerializeField]
    private InputConfig Config { get; set; }


    public Vector2 Direction => throw new NotImplementedException();

    public bool IsActivate => throw new NotImplementedException();


    public event Action<Vector2> OnPressStarted;
    public event Action<Vector2> OnPressConfirmed;
    public event Action<Vector2> OnReleaseStarted;
    public event Action<Vector2> OnReleaseConfirmed;



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
    private Vector2 IntentDirection
    {
        get
        {
            var direction = Intents?.GetIntent();
            return direction.HasValue ? direction.Value : Direction;
        }
    }


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



    // === Method === //


    private void OnEnable()
    {
        RawInput ??= GetComponent<IRawDirectionalInput>();
        if (RawInput == null) { enabled = false; return; }

        RawInput.OnPressed -= PressStart;
        RawInput.OnPressed += PressStart;

        RawInput.OnPressed -= ReleaseStart;
        RawInput.OnPressed += ReleaseStart;
    }

    private void OnDisable()
    {
        if (RawInput == null) { return; }

        RawInput.OnPressed -= PressStart;
        RawInput.OnPressed -= ReleaseStart;
    }


    private void Update()
    {
        var direction = RawDirection;
        if (direction == Vector2.zero) { return; }

        Intents?.SetIntent(direction, Time.unscaledTime);
    }


    private void PressStart()
    {
        ReleaseConfirmID++;

        // Release Confirm 중이 아닐 때에만
        // Press Confirm 허용
        if (!ReleasingFlag.IsBlocked)
        {
            OnPressStarted?.Invoke(RawDirection);
            _ = PressConfirm();
        }
    }


    private async Awaitable PressConfirm()
    {
        try
        {
            await Awaitable.WaitForSecondsAsync(Config.InputDelay, destroyCancellationToken);

            if (enabled)
            {
                // Press가 결정된 타이밍의 Intent 전달
                OnPressConfirmed?.Invoke(IntentDirection);
            }
        }
        finally
        {

        }
    }


    private void ReleaseStart()
    {
        // 1. Delay 이전에 Intent 저장 (시간 경과로 인한 Intent 휘발 방지)
        var direction = IntentDirection;
        // Confirm 이전의 Release
        OnReleaseStarted?.Invoke(direction);

        _ = ReleaseConfirm(ReleaseConfirmID, direction);
    }


    /// <summary>
    /// Release를 실제로 발생시킬지 검토
    /// </summary>
    private async Awaitable ReleaseConfirm(int ID, Vector2 direction)
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
