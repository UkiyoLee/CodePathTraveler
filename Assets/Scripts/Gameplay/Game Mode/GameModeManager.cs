public class GameModeManager : Singleton<GameModeManager>
{
    public GameMode currentGameMode;

    [SerializeField] private GameMode defaultMode = GameMode.Explore;

    protected override void Awake()
    {
        base.Awake();
        currentGameMode = defaultMode;
    }

    private void Start()
    {
        // 广播当前游戏模式
        ApplyMode(currentGameMode);
    }

    /// <summary>
    /// 外部请求入口
    /// </summary>
    /// <param name="newMode"></param>
    public void RequestChangeGameMode(GameMode newMode)
    {
        if (Instance != this) return;

        if (!CanSwitchMode(newMode)) return;

        ApplyMode(newMode);
    }

    public bool CanSwitchMode(GameMode newMode)
    {
        if (currentGameMode == GameMode.Battle) return false;
        return true;
    }

    private void ApplyMode(GameMode newMode)
    {
        currentGameMode = newMode;
        EventBus.Publish(new GameModeChangedEvent(currentGameMode));
    }
}
