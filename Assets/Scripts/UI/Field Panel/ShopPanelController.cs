using System;
using TMPro;
using UnityEngine.UI;

public class ShopPanelController : PanelController
{
    [Header("一级按钮与金额")]
    [SerializeField] private Button buyButton;
    [SerializeField] private Button sellButton;
    [SerializeField] private TMP_Text currencyAmountText;

    [Header("二级列表")]
    [SerializeField] private ItemPanelController itemPanel;

    [Header("交互区域")]
    [SerializeField] private CanvasGroup leftPart;
    [SerializeField] private CanvasGroup itemPanelCanvasGroup;

    [Header("Confirm Popup")]
    [SerializeField] private RectTransform confirmPopup;
    [SerializeField] private TMP_Text popupTitle;
    [SerializeField] private TMP_Text popupText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    public override Type PanelActionType => typeof(ShopAction);

    private PanelType _currentShopType;
    private ItemDefinitionSO _pendingItem;

    private void Awake()
    {
        RebindButtons(buyButton, OpenBuyPanel);
        RebindButtons(sellButton, OpenSellPanel);
        confirmPopup.gameObject.SetActive(false);
    }

    public override void SetupPanel(ActionBase actionBase)
    {
        base.SetupPanel(actionBase);
        SetDefaultSelection();
        UpdateCurrencyDisplay();
    }

    private void OpenBuyPanel()
    {
        OpenItemPanel(PanelType.Buy);
    }

    private void OpenSellPanel()
    {
        OpenItemPanel(PanelType.Sell);
    }

    public void OpenItemPanel(PanelType panelType)
    {
        _currentShopType = panelType;
        leftPart.interactable = false;
        itemPanel.gameObject.SetActive(true);
        itemPanel.SetupPanel(panelType, CurrentAction, OpenConfirmPopup);
    }

    private void OpenConfirmPopup(ItemDefinitionSO itemDefinition)
    {
        _pendingItem = itemDefinition;
        confirmPopup.gameObject.SetActive(true);
        itemPanelCanvasGroup.interactable = false;

        // 装饰弹出窗口
        if (_currentShopType == PanelType.Buy)
        {
            SetupBuyPopup(itemDefinition);
        }
        else
        {
            SetupSellPopup(itemDefinition);
        }
    }

    private void SetupBuyPopup(ItemDefinitionSO itemDefinition)
    {
        bool canAfford = InventoryManager.Instance.Currency >= itemDefinition.buyPrice;

        popupTitle.text = "确定要购买以下物品吗？";
        popupText.text = $"{itemDefinition.itemName}\n价格：￥{itemDefinition.buyPrice}";

        confirmButton.interactable = canAfford;
        (canAfford ? confirmButton : cancelButton).Select();
    }

    private void SetupSellPopup(ItemDefinitionSO itemDefinition)
    {
        popupTitle.text = "确定要出售以下物品吗？";
        popupText.text = $"{itemDefinition.itemName}\n获得：￥{itemDefinition.sellPrice}";
        confirmButton.interactable = true;
        confirmButton.Select();
    }

    public override bool HandleCancelInput()
    {
        if (confirmPopup.gameObject.activeSelf)
        {
            confirmPopup.gameObject.SetActive(false);
            itemPanelCanvasGroup.interactable = true;
            if (itemPanelCanvasGroup.gameObject.activeInHierarchy)
            {
                itemPanel.SetDefaultSelection();
            }
            return true;
        }

        if (!itemPanel.gameObject.activeSelf)
        {
            return false;
        }

        itemPanel.gameObject.SetActive(false);
        leftPart.interactable = true;
        _pendingItem = null;
        FirstButton = _currentShopType == PanelType.Buy ? buyButton : sellButton;
        SetDefaultSelection();
        return true;
    }

    private void UpdateCurrencyDisplay()
    {
        if (currencyAmountText == null) return;

        var inventory = InventoryManager.Instance;
        int currentMoney = inventory.Currency;
        currencyAmountText.text = $"持有金额：￥{currentMoney}";
    }
}
