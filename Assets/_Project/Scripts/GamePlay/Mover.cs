using UnityEngine;

/// <summary>
/// IMover 인터페이스의 기본 구현체
/// </summary>
public class Mover : MonoBehaviour, IMover
{
    [field: SerializeField]
    public float MoveSpeed { get; private set; } = 5f;

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