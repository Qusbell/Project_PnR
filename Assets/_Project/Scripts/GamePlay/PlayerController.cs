using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler), typeof(Mover))]
public class PlayerController : NetworkBehaviour
{
    private PlayerInputHandler _inputHandler;
    private Mover _movement;

    public override void OnNetworkSpawn()
    {
        _inputHandler = GetComponent<PlayerInputHandler>();
        _movement = GetComponent<Mover>();

        // 로컬 플레이어일 때만 입력 시스템 초기화
        if (IsOwner)
        {
            _inputHandler.Initialize();
        }
        else
        {
            // 타인 캐릭터의 입력 핸들러는 비활성화하여 오버헤드 감소
            _inputHandler.enabled = false;
        }
    }

    void Update()
    {
        // 내 캐릭터가 아니면 로직을 실행하지 않음 (네트워크 최적화)
        if (!IsOwner) return;

        // 조율: 입력을 읽어서 이동으로 전달
        _movement.Move(_inputHandler.MoveInput);
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            _inputHandler.DisableInput();
        }
    }
}