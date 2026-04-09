using UnityEngine.UI;
using UnityEngine.Pool;
using MFramework.Event;
using System;
using UnityEngine.EventSystems;

public class InteractionUIController : MonoBehaviour,
IEventReceiver<InteractionChangedEvent>, IEventReceiver<InteractionMenuRequestEvent>, IEventReceiver<GameModeChangedEvent>
{
    [Header("Head Icon")]
    [SerializeField] private RectTransform actionIconHolder;
    [SerializeField] private GameObject actionIconPrefeb;

    [Header("Menu Icon")]
    [SerializeField] private RectTransform actionMenuHolder;
    [SerializeField] private GameObject actionMenuButtonPrefeb;

    private ObjectPool<GameObject> _iconPool;
    private ObjectPool<GameObject> _menuButtonPool;
    private readonly List<GameObject> _activeIcons = new(8);
    private readonly List<GameObject> _activeButtons = new(8);
    private IReadOnlyList<ActionCommandInfo> _currentCommandList;
    private Transform _headAnchor;

    # region 周期函数
    private void Awake()
    {
        InitPool();
        actionIconHolder.gameObject.SetActive(false);
        actionMenuHolder.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        EventBus.Subscribe<InteractionChangedEvent>(this);
        EventBus.Subscribe<InteractionMenuRequestEvent>(this);
        EventBus.Subscribe<GameModeChangedEvent>(this);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<InteractionChangedEvent>(this);
        EventBus.Unsubscribe<InteractionMenuRequestEvent>(this);
        EventBus.Unsubscribe<GameModeChangedEvent>(this);
    }

    private void Update()
    {
        if (GameModeManager.Instance.currentGameMode != GameMode.InteractionMenu) return;
        var input = InputSystemController.Instance;

        if (input.GetUICancelPressed())
        {
            CloseMenu(true);
            GameModeManager.Instance.RequestChangeGameMode(GameMode.Explore);
        }
    }

    private void LateUpdate()
    {
        if (!actionIconHolder.gameObject.activeSelf || _headAnchor == null) return;
        UpdateHeadIconPosition();
    }
    # endregion

    private void UpdateHeadIconPosition()
    {
        var worldPos = _headAnchor.position;
        var screenPos = Camera.main.WorldToScreenPoint(worldPos);

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, screenPos, null, out var localPoint))
        {
            actionIconHolder.anchoredPosition = localPoint;
        }
    }

    private void InitPool()
    {
        _iconPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(actionIconPrefeb, actionIconHolder),
            actionOnGet: icon =>
            {
                icon.SetActive(true);
                icon.transform.SetAsLastSibling();
            },
            actionOnRelease: icon => icon.SetActive(false),
            actionOnDestroy: icon => Destroy(icon),
            defaultCapacity: 8,
            maxSize: 20
        );

        _menuButtonPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(actionMenuButtonPrefeb, actionMenuHolder),
            actionOnGet: menu =>
            {
                menu.SetActive(true);
                menu.transform.SetAsLastSibling();
            },
            actionOnRelease: menu => menu.SetActive(false),
            actionOnDestroy: menu => Destroy(menu),
            defaultCapacity: 8,
            maxSize: 20
        );
    }

    # region 对象池相关方法
    private void SyncPool(List<GameObject> activeList, ObjectPool<GameObject> pool, int targetCount)
    {
        while (activeList.Count > targetCount)
        {
            int lastIndex = activeList.Count - 1;
            GameObject item = activeList[lastIndex];
            pool.Release(item);
            activeList.RemoveAt(lastIndex);
        }

        while (activeList.Count < targetCount)
        {
            GameObject item = pool.Get();
            activeList.Add(item);
        }
    }

    private void ReleaseAll(List<GameObject> activeList, ObjectPool<GameObject> pool)
    {
        for (int i = 0; i < activeList.Count; i++)
        {
            pool.Release(activeList[i]);
        }
        activeList.Clear();
    }
    #endregion

    # region 事件相关方法
    /// <summary>
    /// 启动头顶Icon
    /// </summary>
    /// <param name="evt"></param>
    public void OnEvent(InteractionChangedEvent evt)
    {
        if (!evt.inRange || evt.target is null)
        {
            actionIconHolder.gameObject.SetActive(false);
            ReleaseAll(_activeIcons, _iconPool);
            return;
        }
        // 启动显示头顶Icon
        _currentCommandList = evt.target.CachedActionCommandInfo;
        _headAnchor = evt.target.HeadAnchor;
        ShowHeadIcons();
    }

    /// <summary>
    /// 启动菜单
    /// </summary>
    /// <param name="evt"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void OnEvent(InteractionMenuRequestEvent evt)
    {
        actionIconHolder.gameObject.SetActive(false);
        ReleaseAll(_activeIcons, _iconPool);

        actionMenuHolder.gameObject.SetActive(true);
        OpenMenu(evt.target);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="evt"></param>
    public void OnEvent(GameModeChangedEvent evt)
    {
        if (evt.newMode == GameMode.InteractionMenu) return;

        if (evt.newMode == GameMode.Explore)
        {
            if (_currentCommandList is not null && _currentCommandList.Count > 0)
            {
                ShowHeadIcons();
            }
        }
    }
    #endregion

    private void ShowHeadIcons()
    {
        if (_currentCommandList.Count == 0) return;
        actionIconHolder.gameObject.SetActive(true);

        SyncPool(_activeIcons, _iconPool, _currentCommandList.Count);

        for (int i = 0; i < _activeIcons.Count; i++)
        {
            var obj = _activeIcons[i];
            var cmd = _currentCommandList[i];

            obj.GetComponent<Image>().sprite = cmd.Icon;
        }
    }

    private void OpenMenu(InteractionBase target)
    {
        GameModeManager.Instance.RequestChangeGameMode(GameMode.InteractionMenu);

        SyncPool(_activeButtons, _menuButtonPool, _currentCommandList.Count);

        Button firstButton = null;

        for (int i = 0; i < _activeButtons.Count; i++)
        {
            var btn = _activeButtons[i];
            var cmd = _currentCommandList[i];

            int commandIndex = i;
            btn.GetComponent<ActionMenuButton>().SetButton(cmd, () =>
            {
                target.ExecuteCommandFromUI(commandIndex);
                CloseMenu(false);
            });

            if (firstButton is null)
            {
                firstButton = btn.GetComponent<Button>();
            }
        }

        if (firstButton is not null)
        {
            firstButton.Select();
            EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
        }
    }

    private void CloseMenu(bool restoreHeadIcons)
    {
        HideActionMenu();
        if (restoreHeadIcons)
        {
            ShowHeadIcons();
        }
        else
        {
            HideHeadIcons();
        }
    }

    private void HideHeadIcons()
    {
        actionIconHolder?.gameObject.SetActive(false);
        ReleaseAll(_activeIcons, _iconPool);
    }

    private void HideActionMenu()
    {
        actionMenuHolder?.gameObject.SetActive(false);
        ReleaseAll(_activeButtons, _menuButtonPool);
    }
}