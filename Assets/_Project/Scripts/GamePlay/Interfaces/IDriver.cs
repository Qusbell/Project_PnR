using UnityEngine;

/// <summary>
/// 방향성 이동이 가능한 오브젝트를 표현 <br/>
/// 나침반 방향으로 핸들을 꺾는 운전자
/// </summary>
public interface IDriver
{
    /// <summary>
    /// 나침반 방향에 따른 이동 상태
    /// </summary>
    bool IsMoving { get; }

    /// <summary>
    /// Compass를 연결한다
    /// </summary>
    /// <param name="compass">IDriver가 사용할 나침반</param>
    void MoveBy(ICompass compass);
}