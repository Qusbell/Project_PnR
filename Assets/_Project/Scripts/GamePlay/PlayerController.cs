using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    private InputSystem_Actions _inputActions;
    private Vector2 _moveInput;

    [SerializeField] private float moveSpeed = 5f;

    public override void OnNetworkSpawn()
    {
        // 로컬 플레이어(나)일 때만 입력을 활성화합니다.
        if (IsOwner)
        {
            _inputActions = new InputSystem_Actions();
            _inputActions.Player.Enable();
        }
    }

    void Update()
    {
        // 내 캐릭터가 아니면 입력을 처리하지 않습니다. (멀티플레이어 핵심)
        if (!IsOwner) return;

        // 1. 입력 값 읽기
        _moveInput = _inputActions.Player.Move.ReadValue<Vector2>();

        // 2. 이동 처리
        Vector3 moveDelta = new Vector3(_moveInput.x, _moveInput.y, 0f) * moveSpeed * Time.deltaTime;
        transform.position += moveDelta;
    }

    public override void OnNetworkDespawn()
    {
        // 메모리 누수 방지를 위해 해제
        _inputActions?.Player.Disable();
    }
}