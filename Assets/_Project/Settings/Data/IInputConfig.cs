using UnityEngine;

/// <summary>
/// 입력 속성
/// </summary>
public interface IInputConfig
{
    /// <summary>
    /// 대각선 방향으로의 입력 허용 딜레이
    /// </summary>
    public float DiagonalDelay { get; }

    /// <summary>
    /// Release 발생 시, 이것이 의도된 Release인지 체크할 시간
    /// </summary>
    public float ReleaseDelay { get; }

    /// <summary>
    /// 데드존
    /// </summary>
    public float DeadZone { get; }
}
