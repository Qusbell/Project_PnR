using UnityEngine;

public interface IFlag
{
    /// <summary>
    /// 이미 진입 상태인지 <br/>
    /// 가로막힌 상태 = true
    /// </summary>
    bool IsBlocked { get; }

    /// <summary>
    /// 진입 시도
    /// </summary>
    /// <returns>진입 가능 여부</returns>
    bool TryEnter();

    /// <summary>
    /// 진입 확정
    /// </summary>
    void Enter();

    /// <summary>
    /// 퇴장 확정
    /// </summary>
    void Exit();
}
