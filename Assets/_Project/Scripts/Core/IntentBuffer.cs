using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 입력값을 해석하고, 대각선의 의도가 있었다면 반환
/// (사실상 키보드 쪽으로만 사용될듯?)
/// </summary>
public class IntentBuffer
{
    private Queue<(Vector2, float)> intents = new();

    /// <summary>
    /// 최대 허용되는 delay 길이
    /// </summary>
    private float IntentMaxDelay { get; set; }

    /// <summary>
    /// 최대 허용되는 Queue 길이
    /// (프레임이 너무 빠르면 delay 경과 이전에도 삭제되어 버릴 수도 있음)
    /// </summary>
    private int MaxSize { get; set; }

    /// <summary>
    /// 대각선 의도에 허용되는 최대 deadzone
    /// </summary>
    private float DeadZone { get; set; }

    public IntentBuffer(float delay = 0.1f, int maxSize = 20, float deadZone = 0.5f)
    {
        IntentMaxDelay = delay;
        MaxSize = maxSize;
        DeadZone = deadZone;
    }


    /// <summary>
    /// 입력 의도 넣기
    /// </summary>
    public void SetIntent(Vector2 direction, float time)
    {
        // 큐 넣기
        intents.Enqueue((direction, time));

        // 순환큐
        if (intents.Count > MaxSize)
        {
            intents.Dequeue();
        }
    }


    /// <summary>
    /// 대각선 의도 가져오기. null 반환 = 대각선 의도 없었음
    /// </summary>
    public Vector2? GetIntent()
    {
        Vector2? direction = null;

        while (intents.Count > 0)
        {
            var intent = intents.Dequeue();

            // 무의미 데이터 생략
            if(Time.time - intent.Item2 > IntentMaxDelay) { continue; }
            
            if (Mathf.Abs(intent.Item1.x) > DeadZone &&
                Mathf.Abs(intent.Item1.y) > DeadZone)
            {
                // 가장 최근의 대각선 의도를 찾기 위해 결과값 업데이트
                direction = intent.Item1;
            }
        }

        // while 탈출 후 direction은 가장 최근 입력의 대각선 의도를 갖고 있음

        return direction;
    }

}
