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

    void Update()
    {
        var mode = GameModeManager.Instance.currentGameMode;

        if (mode is GameMode.Battle) return;

        if (mode is GameMode.InteractionMenu)
        {
            if (IsAnyPanelActive() && InputSystemController.Instance.GetUICancelPressed())
            {
                TryHandleCancelByActionPanel();
                return;
            }
        }

        if (InputSystemController.Instance.GetUICancelPressed())
        {
            CloseAllPanels();
        }
    }

    void OnEnable()
    {
        EventBus.Subscribe<PanelRequestEvent>(this);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<PanelRequestEvent>(this);
    }

    # endregion

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

    private void TryHandleCancelByActionPanel()
    {
        foreach (var panel in _allPanelList)
        {
            if (panel.gameObject.activeSelf)
            {
                panel.gameObject.SetActive(false);
                return;
            }
        }
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
