using System.Globalization;
using Unity.Netcode;
using UnityEngine;


[RequireComponent(typeof(IPlayerInput), typeof(IMover))]
public class PlayerController : NetworkBehaviour
{
    private IPlayerInput _inputHandler;
    private IMover _mover;

    private IPlayerInput InputHandler => _inputHandler ??= GetComponent<IPlayerInput>();
    private IMover Mover => _mover ??= GetComponent<IMover>();

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            InputHandler.Initialize();
        }
        else
        {
            if (InputHandler is MonoBehaviour mb) mb.enabled = false;
        }
    }

    void Update()
    {
        if (!IsOwner) return;
        Mover.Move(InputHandler.MoveInput);
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner) InputHandler.DisableInput();
    }
}