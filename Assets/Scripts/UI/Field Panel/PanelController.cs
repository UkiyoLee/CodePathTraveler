using System;
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

    public virtual Type PanelActionType => null;


    public virtual void SetupPanel(ActionBase actionBase)
    {
        CurrentAction = actionBase;
        ActionIcon.sprite = actionBase.CommandInfo.Icon;
    }

    public virtual void ClosePanel()
    {
        gameObject.SetActive(false);
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
