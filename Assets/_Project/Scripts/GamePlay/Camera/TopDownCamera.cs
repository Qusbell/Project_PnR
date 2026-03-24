using UnityEngine;

public class TopDownCamera : MonoBehaviour, ITraveler
{
    // ==== Field ==== //

    [field:SerializeField]
    private CameraConfig Config { get; set; }

    private IDestination Destination { get; set; }

    private Vector3 _currentVelocity;


    // ==== Life Cycle ==== //


    private void LateUpdate()
    {
        UpdateCameraPosition();
    }

    // ==== Custom ==== //

    /// <summary>
    /// SmoothDamp를 이용한 가속도 기반 위치 보간
    /// </summary>
    private void UpdateCameraPosition()
    {
        if (Destination == null || !Destination.IsActivated) { return; }

        Vector3 targetPosition = new Vector3(Destination.Position.x, Destination.Position.y) + Config.Offset;

        // 2D 환경에서도 카메라는 -10 정도 유지되어야 함
        // 사실 내부적으로는 3D를 쓰나?
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref _currentVelocity,
            Config.SmoothTime,
            Config.MaxSpeed
        );
    }

    public void MoveTo(IDestination destination)
    {
        Destination = destination;
    }

    public void Stop()
    {
        Destination = null;
    }


    // ==== Debug ==== //

    private void OnValidate()
    {
        if (Config == null) { Debug.LogError($"{name} : Config = null"); }
    }

}