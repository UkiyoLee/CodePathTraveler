using UnityEngine.UI;
using TMPro;
using System;

public class StealPanelController : PanelController
{
    [Header("Steal Panel")]

    [SerializeField] private StealItemIcon stealItemButtonPrefab;
    [SerializeField] private RectTransform contentRoot;

    [Header("Confirm Popup")]
    [SerializeField] private RectTransform confirmPopup;
    [SerializeField] private TMP_Text popupText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private readonly List<StealItemIcon> _stealItemButtons = new();

    public override Type PanelActionType => typeof(StealAction);
    private StealAction CurrentStealAction => (StealAction)CurrentAction;

    private ItemDefinitionSO _pendingItem;

    public override void SetupPanel(ActionBase actionBase)
    {
        base.SetupPanel(actionBase);

        confirmPopup.gameObject.SetActive(false);
        RefreshItemList();
    }

    private void RefreshItemList()
    {
        ClearItemList();

        foreach (InventoryItem item in CurrentStealAction.itemToSteal)
        {
            StealItemIcon stealItemButton = Instantiate(stealItemButtonPrefab, contentRoot);
            stealItemButton.SetupButton(item, OpenConfirmPopup);
            _stealItemButtons.Add(stealItemButton);
        }

        if (_stealItemButtons.Count == 0)
        {
            return;
        }
        FirstButton = _stealItemButtons[0].CurrentButton;
        SetDefaultSelection();
    }

    private void OpenConfirmPopup(ItemDefinitionSO item)
    {
        _pendingItem = item;
        FirstButton = confirmButton;
        confirmPopup.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(true);

        popupText.text = $"{item.itemName} 成功率 {item.rarityWeight}%";

        SetButtonsInteractable(false);

        RebindButtons(confirmButton, OnConfirm);
        RebindButtons(cancelButton, ClosePopup);
        SetDefaultSelection();
    }

    protected override void OnConfirm()
    {
        var succeed = CurrentStealAction.TrySteal(_pendingItem ?? null);
        popupText.text = succeed ? "偷窃成功" : "偷窃失败";
        cancelButton.gameObject.SetActive(false);
        RebindButtons(confirmButton, ClosePopup);
    }

    private void ClosePopup()
    {
        HidePopup();
        RefreshItemList();
    }

    private void HidePopup()
    {
        _pendingItem = null;
        confirmPopup.gameObject.SetActive(false);
        SetButtonsInteractable(true);
    }

    public override bool HandleCancelInput()
    {
        if (confirmPopup.gameObject.activeSelf)
        {
            HidePopup();
            return true;
        }
        return false;
    }

    private void SetButtonsInteractable(bool interactable)
    {
        foreach (StealItemIcon stealItemButton in _stealItemButtons)
        {
            stealItemButton.CurrentButton.interactable = interactable;
        }
    }

    private void ClearItemList()
    {
        foreach (StealItemIcon stealItemButton in _stealItemButtons)
        {
            Destroy(stealItemButton.gameObject);
        }
        _stealItemButtons.Clear();

        FirstButton = null;
    }
}
