using UnityEngine;

public interface IFlag
{
    /// <summary>
    /// 이미 진입 상태인지
    /// </summary>
    bool IsFlagOn { get; }

    /// <summary>
    /// 진입 시도
    /// </summary>
    /// <returns>진입 가능 여부</returns>
    bool TryEnter();

    /// <summary>
    /// 퇴장
    /// </summary>
    void Exit();
}
