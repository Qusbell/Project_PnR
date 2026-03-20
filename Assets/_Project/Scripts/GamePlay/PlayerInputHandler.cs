using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour, IPlayerInput
{
    [SerializeField] private PlayerInputConfig _config;

    private @InputSystem_Actions _inputActions;
    private Vector2 _moveInput;
    private float _chargeStartTime;
    private bool _isCharging;

    private @InputSystem_Actions InputActions => _inputActions ??= new @InputSystem_Actions();

    public Vector2 MoveInput => _moveInput;
    public bool IsCharging => _isCharging;
    public float ChargeRatio => _isCharging
        ? Mathf.Clamp01((Time.time - _chargeStartTime) / _config.MaxChargeTime)
        : 0f;

    public event Action<float> OnAttackReleased;

    public void Initialize()
    {
        InputActions.Player.Enable();
        InputActions.Player.Attack.started += StartCharge;
        InputActions.Player.Attack.canceled += ReleaseCharge;
    }

    public void DisableInput()
    {
        // 상태 초기화 및 이벤트 해제
        _isCharging = false;
        if (_inputActions == null) return;

        InputActions.Player.Attack.started -= StartCharge;
        InputActions.Player.Attack.canceled -= ReleaseCharge;
        InputActions.Player.Disable();
    }

    private void OnDisable() => DisableInput();

    private void Update()
    {
        _moveInput = InputActions.Player.Move.ReadValue<Vector2>();
    }

    private void StartCharge(InputAction.CallbackContext context)
    {
        _isCharging = true;
        _chargeStartTime = Time.time;
    }

    private void ReleaseCharge(InputAction.CallbackContext context)
    {
        if (!_isCharging) return;

        float duration = Time.time - _chargeStartTime;
        _isCharging = false;

        if (duration >= _config.MinChargeThreshold)
        {
            float finalCharge = Mathf.Clamp01(duration / _config.MaxChargeTime);
            OnAttackReleased?.Invoke(finalCharge);
        }
    }
}