using UnityEngine;

public class Mover : MonoBehaviour, IMover
{
    [SerializeField]
    private float _moveSpeed = 5f;

    public float MoveSpeed => _moveSpeed;

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