using Unity.Netcode;
using UnityEngine;

/// <summary>
/// IMover 인터페이스의 기본 구현체
/// </summary>
public class Mover : NetworkBehaviour, IMovable
{
    // ==== Field ====

    /// <summary>
    /// <-- 스크립터블이든 NetVariable이든 뭐든 나중에 따로 빼둬야 할듯?
    /// </summary>
    [field: SerializeField]
    public float MoveSpeed { get; private set; } = 5f;

    // ==== Custom ====

    /// <summary>
    /// 입력된 방향으로 오브젝트를 이동시킵니다.
    /// </summary>
    public void Move(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.01f)
        {
            return;
        }

        Vector3 moveDelta = new Vector3(direction.x, direction.y, 0f) * MoveSpeed * Time.deltaTime;
        transform.position += moveDelta;
    }


}