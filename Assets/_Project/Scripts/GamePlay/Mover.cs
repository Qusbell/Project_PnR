using UnityEngine;
using Unity.Netcode;

public class Mover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    // 이동 로직
    public void Move(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.01f) return;

        Vector3 moveDelta = new Vector3(direction.x, direction.y, 0f) * moveSpeed * Time.deltaTime;
        transform.position += moveDelta;
    }
}