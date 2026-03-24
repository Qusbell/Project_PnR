using UnityEngine;

/// <summary>
/// 방향성 이동이 가능한 오브젝트를 표현 <br/>
/// 방향을 받아 핸들을 꺾는 운전자
/// </summary>
public interface IDriver
{
    void MoveAt(Vector2 direction);
}