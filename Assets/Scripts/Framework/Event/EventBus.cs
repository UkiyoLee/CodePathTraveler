using System;
using MFramework.Event;

public static class EventBus
{
    private static readonly Dictionary<Type, List<object>> EventDict = new();


    /// <summary>
    /// 订阅事件的方法
    /// </summary>
    /// <typeparam name="TEvent">事件类型，必须实现IEvent接口</typeparam>
    /// <param name="receiver">事件接收者，必须实现IEventReceiver<TEvent>接口</param>
    public static void Subscribe<TEvent>(IEventReceiver<TEvent> receiver) where TEvent : IEvent
    {
        // 获取事件类型的Type对象
        Type eventType = typeof(TEvent);

        // 检查事件字典中是否已存在该事件类型的接收者列表
        if (!EventDict.TryGetValue(eventType, out var receivers))
        {
            // 如果不存在，则创建一个新的接收者列表并添加到字典中
            receivers = new List<object>();
            EventDict[eventType] = receivers;
        }

        // 检查接收者是否已在当前事件的接收者列表中，避免重复订阅
        if (!receivers.Contains(receiver))
        {
            // 如果不存在，则将接收者添加到列表中
            receivers.Add(receiver);
        }
    }


    /// <summary>
    /// 取消订阅指定类型的事件
    /// </summary>
    /// <typeparam name="TEvent">事件类型，必须实现IEvent接口</typeparam>
    /// <param name="receiver">事件接收者，用于处理事件的对象</param>
    public static void Unsubscribe<TEvent>(IEventReceiver<TEvent> receiver) where TEvent : IEvent
    {
        // 获取事件类型的Type对象
        Type eventType = typeof(TEvent);

        // 检查事件字典中是否已存在该事件类型的接收者列表
        if (EventDict.TryGetValue(eventType, out var receivers))
        {
            // 如果存在，则从列表中移除接收者
            receivers.Remove(receiver);

            // 如果列表为空，则从字典中移除该事件类型的键值对
            if (receivers.Count == 0)
            {
                EventDict.Remove(eventType);
            }
        }
    }


    /// <summary>
    /// 发布事件的方法，用于将事件通知给所有已注册的事件接收者
    /// </summary>
    /// <typeparam name="TEvent">事件类型，必须实现IEvent接口</typeparam>
    /// <param name="evt">要发布的事件实例</param>
    public static void Publish<TEvent>(TEvent evt) where TEvent : IEvent
    {
        // 获取事件类型的Type对象，用于在字典中查找对应的接收者列表
        Type eventType = typeof(TEvent);

        // 检查事件字典中是否已存在该事件类型的接收者列表
        // TryGetValue可以避免两次查找，提高性能
        if (EventDict.TryGetValue(eventType, out var receivers))
        {
            // 遍历所有接收者
            for (int i = 0; i < receivers.Count; i++)
            {
                // 获取当前接收者对象
                var obj = receivers[i];

                // 检查Unity对象是否已被销毁
                // unityObj == null 检查的是Unity对象的实例是否已被销毁
                if (obj is UnityEngine.Object unityObj && unityObj == null)
                {
                    // 如果Unity对象已被销毁，则从列表中移除该接收者
                    receivers.RemoveAt(i);
                    // 因为移除了元素，需要调整索引
                    i--;
                    continue;
                }

                // 调用接收者的OnEvent方法处理事件
                // 这里使用强制类型转换，因为前面已经确保了对象类型正确
                ((IEventReceiver<TEvent>)receivers[i]).OnEvent(evt);
            }
            // 如果所有接收者都被移除了，则从字典中删除该事件类型
            if (receivers.Count == 0)
            {
                EventDict.Remove(eventType);
            }

            if (receivers.Count == 0)
            {
                EventDict.Remove(eventType);
            }
        }
    }
}