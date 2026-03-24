using UnityEngine;

public class CameraTarget_Test : MonoBehaviour, IDestination
{
    public Vector2 Position => transform.position;

    public bool IsActivated => enabled;

    private ITraveler targetCamera;


    // <-- IsNetOwner 검사 필요


    private void OnEnable()
    {
        if(Camera.main.TryGetComponent<ITraveler>(out targetCamera))
        {
            targetCamera.MoveTo(this);
        }
    }

    private void OnDisable()
    {
        targetCamera?.Stop();
    }

}
