using MFramework.Event;

public readonly struct InteractionChangedEvent : IEvent
{
    public readonly InteractionBase target;
    public readonly bool inRange;

    /// <summary>
    /// 交互提示变化事件
    /// </summary>
    /// <param name="target"></param>
    /// <param name="inRange">inRange = false 或无可用命令：隐藏头顶Icon + 关闭菜单</param>
    public InteractionChangedEvent(InteractionBase target, bool inRange)
    {
        this.target = target;
        this.inRange = inRange;
    }
}

public readonly struct InteractionMenuRequestEvent : IEvent
{
    public readonly InteractionBase target;

    public InteractionMenuRequestEvent(InteractionBase target)
    {
        this.target = target;
    }
}