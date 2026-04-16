using UnityEngine.InputSystem;
using MFramework.Event;

public class InputSystemController : Singleton<InputSystemController>, IEventReceiver<GameModeChangedEvent>
{
    private CharacterInputActions _inputActions;

    public CharacterInputActions InputActions => _inputActions;

    private ActiveMap _currentMap = ActiveMap.None;

    private bool _isInitialized = false;

    protected override void Awake()
    {
        base.Awake();
        if (!_isInitialized)
        {
            _inputActions ??= new CharacterInputActions();
            _isInitialized = true;
        }
    }

    void OnEnable()
    {
        EventBus.Subscribe<GameModeChangedEvent>(this);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<GameModeChangedEvent>(this);
    }

    void OnDestroy()
    {
        _inputActions?.Dispose();
    }


    public Vector2 GetMovementInput()
    {
        if (!_isInitialized || _currentMap != ActiveMap.Player)
        {
            // Debug.Log("InputSystemController not initialized or current map is not player");
            return Vector2.zero;
        }

        return _inputActions.Player.Move.ReadValue<Vector2>();
    }

    public bool GetPlayerConfirmPressed()
    {
        if (!_isInitialized || _currentMap != ActiveMap.Player)
            return false;
        return _inputActions.Player.Confirm.WasPressedThisFrame();
    }

    public bool GetUICancelPressed()
    {
        if (!_isInitialized || _currentMap != ActiveMap.UI)
            return false;
        return _inputActions.UI.Cancel.WasPressedThisFrame();
    }


    #region 事件实现
    public void OnEvent(GameModeChangedEvent evt)
    {
        // Debug.Log(evt.newMode);
        _currentMap = GetMapFromGameMode(evt.newMode);

        switch (_currentMap)
        {
            case ActiveMap.Player:
                _inputActions.Player.Enable();
                _inputActions.UI.Disable();
                break;
            case ActiveMap.UI:
                _inputActions.Player.Disable();
                _inputActions.UI.Enable();
                break;
            case ActiveMap.None:
            default:
                break;
        }
    }

    private ActiveMap GetMapFromGameMode(GameMode gameMode)
    {
        switch (gameMode)
        {
            case GameMode.Battle:
            case GameMode.InteractionMenu:
            case GameMode.Pause:
                return ActiveMap.UI;
            case GameMode.Explore:
            default:
                return ActiveMap.Player;
        }
    }

    #endregion
}