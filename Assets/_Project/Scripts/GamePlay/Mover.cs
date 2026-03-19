using UnityEngine;
using Unity.Netcode;

public class Mover : MonoBehaviour, IMover
{
    [SerializeField] private float moveSpeed = 5f;

    public float MoveSpeed => moveSpeed;

    // 이동 로직
    public void Move(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.01f) return;

        Vector3 moveDelta = new Vector3(direction.x, direction.y, 0f) * MoveSpeed * Time.deltaTime;
        transform.position += moveDelta;
    }
}