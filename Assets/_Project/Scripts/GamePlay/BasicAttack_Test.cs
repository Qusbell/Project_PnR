using UnityEngine;

public class BasicAttack_Test : PnRAction
{
    // ==== Custom ====

    protected override void OnPressStarted(Vector2 direction)
    {
        startTime_Test = Time.time;
        Debug.Log($"{name} : OnPressStarted");
    }


    protected override void OnPressConfirmed(Vector2 direction)
    {
        Debug.Log($"{name} : OnPressConfirmed : {direction} 방향");
    }


    protected override void OnReleasStarted(Vector2 direction)
    {
        entTime_Test = Time.time;
        Debug.Log($"{name} : OnReleasStarted : {(int)((entTime_Test - startTime_Test) / chargingTime_Test)}단계 충전");
    }

    protected override void OnReleaseConfirmed(Vector2 direction)
    {
        Debug.Log($"{name} : OnReleaseConfirmed : {direction} 방향");

    }


    // ==== Test ====

    // 단계 증가 시간
    private float chargingTime_Test = 1f;

    // 차지가 시작된 시간
    private float startTime_Test = 0f;

    private float entTime_Test = 0f;

}
