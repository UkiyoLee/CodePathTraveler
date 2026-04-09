using MFramework.Event;

public class UIManager : MonoBehaviour, IEventReceiver<PanelRequestEvent>
{
    [Header("根节点与特殊面板引用")]
    [SerializeField, Tooltip("探索模式下显示的总体UI根节点")]
    private GameObject fieldUIroot;

    void OnEnable()
    {
        EventBus.Subscribe<PanelRequestEvent>(this);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<PanelRequestEvent>(this);
    }

    public InquirePanelController inquirePanelController;
    public RecruitPanelController recruitPanelController;

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

    private void TryHandleCancelByActionPanel()
    {
        if (inquirePanelController.gameObject.activeSelf)
        {
            inquirePanelController.ClosePanel();
        }
        if (recruitPanelController.gameObject.activeSelf)
        {
            recruitPanelController.ClosePanel();
        }
    }

    private bool IsAnyPanelActive()
    {
        return inquirePanelController.gameObject.activeSelf || recruitPanelController.gameObject.activeSelf;
    }

    private void CloseAllPanels()
    {
        inquirePanelController.gameObject.SetActive(false);
        recruitPanelController.gameObject.SetActive(false);
    }

    #region 事件函数
    public void OnEvent(PanelRequestEvent evt)
    {
        if (evt.actionBase is InquireAction)
        {
            inquirePanelController.gameObject.SetActive(true);
            inquirePanelController.SetupPanel(evt.actionBase);
        }

        if (evt.actionBase is RecruitAction)
        {
            recruitPanelController.gameObject.SetActive(true);
            recruitPanelController.SetupPanel(evt.actionBase);
        }
    }
    #endregion
}
