using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerController : MonoBehaviour
{
    private IPlayerInput _input;
    private IMover _mover; // 기존 Mover 시스템 활용

    [SerializeField] private PlayerInputConfig _config;
    private bool _isAttacking;

    // Lazy Initialization
    private IPlayerInput Input => _input ??= GetComponent<IPlayerInput>();
    private IMover Mover => _mover ??= GetComponent<IMover>();

    private void Start()
    {
        Input.Initialize();
        Input.OnAttackReleased += (ratio) => _ = ExecuteAttackAsync(ratio);
    }

    private void Update()
    {
        if (_isAttacking)
        {
            // 공격 중에는 이동 멈춤 (필요 시)
            Mover.Move(Vector2.zero);
            return;
        }

        // 기존 Mover 컴포넌트의 Move 메서드 호출
        Mover.Move(Input.MoveInput);
    }

    private async Awaitable ExecuteAttackAsync(float chargeRatio)
    {
        _isAttacking = true;

        float damage = _config.BaseDamage * (1f + (chargeRatio * (_config.MaxDamageMultiplier - 1f)));
        Debug.Log($"[Attack] Power: {chargeRatio * 100:F0}% | Damage: {damage:F1}");

        // Unity 6 정식 Awaitable API 사용
        await Awaitable.FixedUpdateAsync();

        // 공격 후딜레이 (충전량에 비례)
        await Awaitable.WaitForSecondsAsync(0.3f + (chargeRatio * 0.2f));

        _isAttacking = false;
    }
}