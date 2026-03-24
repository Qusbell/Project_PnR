using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 입력값을 저장하고 일정 시간 내의 대각선 입력 의도를 분석하는 버퍼 클래스
/// </summary>
public class IntentBuffer
{
    private readonly List<(Vector2 Direction, float Time)> _intents = new(64);
    private List<(Vector2 Direction, float Time)> Intents => _intents;

    public float IntentMaxDelay { get; private set; }

    public float DeadZone { get; private set; }


    public IntentBuffer(float delay = 0.1f, float deadZone = 0.5f)
    {
        IntentMaxDelay = delay;
        DeadZone = deadZone;
    }

    /// <summary>
    /// 새로운 입력 의도를 리스트 끝에 추가
    /// </summary>
    public void SetIntent(Vector2 direction, float time)
    {
        Intents.Add((direction, time));
        CleanUp(time);
    }

    /// <summary>
    /// 리스트를 보존하며 유효 시간 내의 가장 최근 대각선 의도를 반환
    /// </summary>
    public Vector2? GetIntent()
    {
        Vector2? latestDiagonal = null;
        float currentTime = Time.unscaledTime;

        // 가장 최근 데이터부터 역순으로 탐색하여 최신성 보장
        for (int i = Intents.Count - 1; i >= 0; i--)
        {
            var (direction, time) = Intents[i];

            // 유효 시간이 지난 데이터라면 (역순이므로 이전 데이터들도 모두 만료임)
            if (currentTime - time > IntentMaxDelay)
            {
                break;
            }

            // 대각선 입력 여부 판단 (DeadZone 기준)
            if (Mathf.Abs(direction.x) > DeadZone &&
                Mathf.Abs(direction.y) > DeadZone)
            {
                latestDiagonal = direction;
                break; // 가장 최근 것을 찾았으므로 즉시 중단
            }
        }

        return latestDiagonal;
    }

    /// <summary>
    /// 시간 순서로 정렬되어 있음을 보장하므로, 앞에서부터 유효하지 않은 구간까지 한 번에 제거합니다.
    /// </summary>
    private void CleanUp(float currentTime)
    {
        int removeCount = 0;

        // 앞에서부터 검사하여 만료된 요소의 개수를 파악
        for (int i = 0; i < Intents.Count; i++)
        {
            if (currentTime - Intents[i].Time > IntentMaxDelay)
            {
                removeCount++;
            }
            else
            {
                // 시간 순이므로 한 번 유효한 데이터가 나오면 이후는 모두 유효함
                break;
            }
        }

        if (removeCount > 0)
        {
            Intents.RemoveRange(0, removeCount);
        }
    }
}