using MFramework.Event;

public class CameraModeController : MonoBehaviour, IEventReceiver<GameModeChangedEvent>
{
    [Header("Camera")]
    [SerializeField] private GameObject followCamera;
    [SerializeField] private GameObject battleCamera;

    private void OnEnable()
    {
        EventBus.Subscribe<GameModeChangedEvent>(this);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<GameModeChangedEvent>(this);
    }

    public void OnEvent(GameModeChangedEvent evt)
    {
        switch (evt.newMode)
        {
            case GameMode.Explore:
                SetCameraView(CameraView.Explore);
                break;
            case GameMode.Battle:
                SetCameraView(CameraView.Battle);
                break;
        }
    }

    public void SetCameraView(CameraView view)
    {
        var followActive = false;
        var battleActive = false;

        switch (view)
        {
            case CameraView.Explore:
                followActive = true;
                break;
            case CameraView.Battle:
                battleActive = true;
                break;
        }

        followCamera.SetActive(followActive);
        battleCamera.SetActive(battleActive);
    }
}
