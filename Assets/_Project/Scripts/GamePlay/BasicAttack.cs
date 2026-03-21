using UnityEngine;

public class BasicAttack : PnRAction
{
    // ==== Custom ====

    protected override void OnPressed(float time)
    {
        Debug.Log($"{name} : Press");
        startTime_Test = time;
    }


    protected override void OnReleased(float time, Vector2 direction)
    {
        Debug.Log($"{name} : Release : {direction}\n {(int)((time - startTime_Test) / chargingTime_Test)}단계 충전");
    }


    // ==== Test ====

    // 단계 증가 시간
    private float chargingTime_Test = 1f;

    // 차지가 시작된 시간
    private float startTime_Test = 0f;

}
