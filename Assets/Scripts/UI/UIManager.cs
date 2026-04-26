using System;
using MFramework.Event;

public class UIManager : MonoBehaviour, IEventReceiver<PanelRequestEvent>
{
    [Header("根节点与特殊面板引用")]
    [SerializeField, Tooltip("探索模式下显示的总体UI根节点")]
    private GameObject fieldUIroot;

    private readonly Dictionary<Type, PanelController> _panelControllerDict = new();
    private readonly List<PanelController> _allPanelList = new();

    # region 周期函数调用

    private void Awake()
    {
        _panelControllerDict.Clear();
        _allPanelList.Clear();

        GetPanelFromRoot(transform);
    }

    void OnEnable()
    {
        EventBus.Subscribe<PanelRequestEvent>(this);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<PanelRequestEvent>(this);
    }

    void Update()
    {
        var mode = GameModeManager.Instance.currentGameMode;
        var input = InputSystemController.Instance;

        if (mode is GameMode.Battle) return;

        if (input.GetUICancelPressed())
        {
            HandleGlobalCancelInput(mode);
        }
    }
    # endregion

    /// <summary>
    /// 处理全局取消输入事件
    /// 此方法用于响应全局取消操作，根据当前活动面板的状态来决定是否关闭所有面板
    /// </summary>
    private void HandleGlobalCancelInput(GameMode currentMode)
    {
        // 首先尝试通过活动面板处理取消操作
        // 如果成功处理，则直接返回，不再执行后续逻辑
        if (TryHandleCancelByActivePanel())
        {
            return;
        }

        // 检查是否有任何面板处于活动状态
        // 如果有活动面板，则关闭所有面板
        if (IsAnyPanelActive())
        {
            CloseAllPanels();
        }

        if (currentMode is GameMode.InteractionMenu)
        {
            GameModeManager.Instance.RequestChangeGameMode(GameMode.Explore);
        }
    }

    private void GetPanelFromRoot(Transform root)
    {
        var panels = root.GetComponentsInChildren<PanelController>(true);

        foreach (var panel in panels)
        {
            _allPanelList.Add(panel);
            if (panel.PanelActionType is not null)
            {
                _panelControllerDict.Add(panel.PanelActionType, panel);
            }
        }
    }

    private bool TryHandleCancelByActivePanel()
    {
        foreach (var panel in _allPanelList)
        {
            if (!panel.gameObject.activeSelf)
            {
                continue;
            }
            if (panel.HandleCancelInput())
            {
                return true;
            }
        }
        return false;
    }

    private bool IsAnyPanelActive()
    {
        foreach (var panel in _allPanelList)
        {
            if (panel.gameObject.activeSelf)
            {
                return true;
            }
        }

        return false;
    }

    private void CloseAllPanels()
    {
        foreach (var panel in _allPanelList)
        {
            panel.gameObject.SetActive(false);
        }
    }

    #region 事件函数
    public void OnEvent(PanelRequestEvent evt)
    {
        var panelType = evt.actionBase.GetType();

        _panelControllerDict.TryGetValue(panelType, out var panelController);
        panelController?.gameObject.SetActive(true);
        panelController?.SetupPanel(evt.actionBase);
    }
    #endregion
}
