public class InteractionBase : MonoBehaviour
{
    [Header("Sign Trans")]
    public Transform HeadAnchor;
    private AllyDefinitionSO _currentInteractor;
    private ActionBase[] _actionsCache;

    private readonly List<ActionCommandInfo> _cachedActionCommandInfo = new(8);
    private readonly List<VisibleActionEntry> _visibleEntries = new(8);
    public IReadOnlyList<ActionCommandInfo> CachedActionCommandInfo => _cachedActionCommandInfo;

    private struct VisibleActionEntry
    {
        public ActionBase Action;
        public ActionCommandInfo CommandInfo;
    }

    private void Awake()
    {
        CacheActions();
        HeadAnchor = transform.GetChild(0);
    }

    /// <summary>
    /// 交互
    /// </summary>
    public void Interact(AllyDefinitionSO interator)
    {
        // PublishEvent(true);
        EventBus.Publish(new InteractionMenuRequestEvent(this));
    }

    /// <summary>
    /// 聚焦时
    /// </summary>
    public void OnFocus(AllyDefinitionSO interator)
    {
        CacheActions();

        _currentInteractor = interator;
        Debug.Log($"OnFocus: ${interator.characterName}, ${interator.Job}");

        RebuildCommands();
        PublishEvent(true);
    }

    /// <summary>
    /// 失去焦点时
    /// </summary>
    public void OnUnfocus(AllyDefinitionSO interator)
    {
        _currentInteractor = null;
        _cachedActionCommandInfo.Clear();
        Debug.Log("On Unfocus");
        PublishEvent(false);
        HeadAnchor.gameObject.SetActive(true);
    }

    private void CacheActions() => _actionsCache = GetComponents<ActionBase>();

    /// <summary>
    /// 重建命令列表的方法
    /// 此方法用于清理并重新构建可见的命令列表，按照命令顺序进行排序
    /// </summary>
    private void RebuildCommands()
    {
        // 清空缓存的动作命令信息和可见条目列表
        _cachedActionCommandInfo.Clear();
        _visibleEntries.Clear();

        // 遍历所有缓存的动作
        for (int i = 0; i < _actionsCache.Length; i++)
        {
            var action = _actionsCache[i];

            // TODO: 之后改成Party Member检测
            if (!action.CanShow(_currentInteractor)) continue;

            _visibleEntries.Add(new VisibleActionEntry
            {
                Action = action,
                CommandInfo = action.CommandInfo
            });
        }

        if (_visibleEntries.Count > 1)
        {
            _visibleEntries.Sort((a, b) => a.CommandInfo.Order.CompareTo(b.CommandInfo.Order));
        }

        for (int i = 0; i < _visibleEntries.Count; i++)
        {
            _cachedActionCommandInfo.Add(_visibleEntries[i].CommandInfo);
        }

        if (_visibleEntries.Count > 0)
        {
            HeadAnchor.gameObject.SetActive(false);
        }
    }

    private void PublishEvent(bool inRange)
    {
        EventBus.Publish(new InteractionChangedEvent(this, inRange));
    }

    # region UI回调入口
    /// <summary>
    /// UI调用
    /// </summary>
    /// <param name="commandIndex"></param>
    /// <returns></returns>
    public bool ExecuteCommandFromUI(int commandIndex)
    {
        if (commandIndex >= _visibleEntries.Count || commandIndex < 0) return false;

        var action = _visibleEntries[commandIndex].Action;

        if (!action.CanExecute(_currentInteractor)) return false;

        action.TriggerAction(_currentInteractor);
        return true;
    }
    # endregion
}
