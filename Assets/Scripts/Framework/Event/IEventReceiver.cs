namespace MFramework.Event;

public interface IEventReceiver<T> where T : IEvent
{
    void OnEvent(T evt);
}
