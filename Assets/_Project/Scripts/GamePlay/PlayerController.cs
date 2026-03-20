using Unity.Netcode;
using UnityEngine;

/// <summary>
/// 플레이어의 입력과 이동 로직을 연결하는 네트워크 컨트롤러
/// </summary>
[RequireComponent(typeof(IPlayerInput), typeof(IMover))]
public class PlayerController : NetworkBehaviour
{
    private IPlayerInput _inputHandler;
    private IPlayerInput InputHandler => _inputHandler ??= GetComponent<IPlayerInput>();

    private IMover _mover;
    private IMover Mover => _mover ??= GetComponent<IMover>();

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            InputHandler?.Initialize();
        }
        else
        {
            // 인터페이스가 MonoBehaviour인 경우 컴포넌트 비활성화
            if (InputHandler is MonoBehaviour inputComponent)
            {
                inputComponent.enabled = false;
            }
        }
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        Mover.Move(InputHandler.MoveInput);
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            InputHandler?.DisableInput();
        }
    }
}