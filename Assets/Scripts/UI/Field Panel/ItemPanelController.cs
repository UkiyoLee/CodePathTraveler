using System;

public class ItemPanelController : PanelController
{
    [Header("Item Panel")]
    [SerializeField] private ShopItemButton itemButtonPrefeb;
    [SerializeField] private Transform itemButtonParent;
    private readonly List<ItemButton> itemButtons = new();
    private PanelType _currentPanelType;
    private Action<ItemDefinitionSO> _onItemClick;

    public void SetupPanel(PanelType panelType, ActionBase actionBase = null, Action<ItemDefinitionSO> onItemClick = null)
    {
        if (actionBase != null)
        {
            base.SetupPanel(actionBase);
        }

        _currentPanelType = panelType;
        _onItemClick = onItemClick;

        ClearItemListView();

        var inventoryManager = InventoryManager.Instance;

        switch (panelType)
        {
            case PanelType.Buy:
                BuildBuyList(inventoryManager);
                break;
            case PanelType.Sell:
                break;
            case PanelType.Item:
                break;
        }
    }

    private void ClearItemListView()
    {
        foreach (var itemButton in itemButtons)
        {
            Destroy(itemButton.CurrentButton.gameObject);
        }
        itemButtons.Clear();
        FirstButton = null;
    }

    private void BuildBuyList(InventoryManager inventory)
    {

    }
}
