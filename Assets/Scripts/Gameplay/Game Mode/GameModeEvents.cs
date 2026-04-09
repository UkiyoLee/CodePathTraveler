using MFramework.Event;

public readonly struct GameModeChangedEvent : IEvent
{
    public readonly GameMode newMode;
    public GameModeChangedEvent(GameMode gameMode)
    {
        this.newMode = gameMode;
    }
}