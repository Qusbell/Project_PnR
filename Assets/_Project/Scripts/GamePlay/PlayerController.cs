using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerController : MonoBehaviour
{
    private IPlayerInput _input;
    [SerializeField] private PlayerInputConfig _config;
    private bool _isAttacking;

    private IPlayerInput Input => _input ??= GetComponent<IPlayerInput>();

    private void Start()
    {
        Input.Initialize();
        Input.OnAttackReleased += (ratio) => _ = ExecuteAttackAsync(ratio);
    }

    private void Update()
    {
        if (_isAttacking) { return; }
        HandleMovement();
    }

    private void HandleMovement()
    {
        // 이동 로직 (기존 Mover 활용 가능)
        if (Input.MoveInput.sqrMagnitude > 0.01f)
        {
            transform.Translate(Input.MoveInput * (Time.deltaTime * 5f));
        }
    }

    /// <summary>
    /// 충전된 비율에 따라 공격을 수행하고 Unity 6 Awaitable로 대기합니다.
    /// </summary>
    private async Awaitable ExecuteAttackAsync(float chargeRatio)
    {
        _isAttacking = true;

        float damage = _config.BaseDamage * (1f + (chargeRatio * (_config.MaxDamageMultiplier - 1f)));
        Debug.Log($"[Attack] Charge: {chargeRatio * 100:F0}% | Damage: {damage:F1}");

        // 공격 애니메이션 및 판정 로직이 들어갈 자리
        // 예: 
        await Awaitable.FixedUpdateAsync(); // 물리 판정 시점 대기

        // 공격 후딜레이 (충전량이 많을수록 후딜레이 증가 예시)
        await Awaitable.WaitForSecondsAsync(0.3f + (chargeRatio * 0.2f));

        _isAttacking = false;
    }
}