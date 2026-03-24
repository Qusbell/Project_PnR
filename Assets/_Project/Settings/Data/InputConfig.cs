using System;
using UnityEngine;

/// <summary>
/// 입력 설정
/// </summary>
[Serializable]
[CreateAssetMenu(fileName = "InputConfig", menuName = "PnR/Config/Input")]
public class InputConfig : ScriptableObject
{
    /// <summary>
    /// 대각선 방향으로의 입력 허용 시간 <br/>
    /// 예: 0.1f값이라면, 0.1초 이내의 대각선 입력이 실제 입력 의도로써 적용될 수 있음
    /// </summary>
    [Header("Input Delay")]
    [field: SerializeField]
    public float DiagonalDelay { get; private set; } = 0.08f;

    /// <summary>
    /// 방향 전환 과정에서 잘못된 Release를 취소할 시간 <br/>
    /// 예: ←를 누르다가 →를 눌렀는데, 그 과정에서 아주 잠시 Release가 발생할 수 있음 <br/>
    /// 이 때 해당 값(sec) 이하만큼 떨어져 있었다면 Release하지 않은 것으로 간주
    /// <br/> <br/>
    /// 2026 03 24 추가: PressConfirmed + ReleaseConfirmed 양쪽 모두 영향 받음
    /// </summary>
    [field: SerializeField]
    public float InputDelay { get; private set; } = 0.05f;


    [Header("DeadZone")]
    [field: SerializeField]
    public float DeadZone { get; private set; } = 0.5f;
}