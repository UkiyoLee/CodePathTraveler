using System;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    [Header("Action")]
    public ActionBase CurrentAction;

    [Header("Focus Navigation")]
    public Button FirstButton;

    [Header("Action Icon")]
    [SerializeField] private Image ActionIcon;


    private DOTweenAnimation _animation;

    public virtual Type PanelActionType => null;

    # region 生命周期函数

    public void OnEnable()
    {
        _animation = GetComponent<DOTweenAnimation>();
        if (!_animation) return;
        _animation.DOPlayForward();
    }

    # endregion


    public virtual void SetupPanel(ActionBase actionBase)
    {
        CurrentAction = actionBase;
        ActionIcon.sprite = actionBase.CommandInfo.Icon;
    }

    public virtual void ClosePanel()
    {
        if (_animation)
        {
            _animation.DOPlayBackwards();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    protected void OnConfirm()
    {
        CurrentAction.Execute();
        ClosePanel();
    }

    protected void OnCancel()
    {
        GameModeManager.Instance.RequestChangeGameMode(GameMode.Explore);
        ClosePanel();
    }

    protected void SetDefaultSelection()
    {
        FirstButton.Select();
        EventSystem.current.SetSelectedGameObject(FirstButton.gameObject);
    }

    public void RebindButtons(Button button, UnityAction unityAction)
    {
        if (button is null) return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(unityAction);
    }
}
