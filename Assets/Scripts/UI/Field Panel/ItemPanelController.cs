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
                BuildSellList(inventoryManager);
                break;
            case PanelType.Item:
                break;
        }

        if (itemButtons.Count > 0)
        {
            FirstButton = itemButtons[0].CurrentButton;
        }
        else
        {
            FirstButton = null;
        }
        SetDefaultSelection();
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
        ShopAction shopAction = (ShopAction)CurrentAction;

        foreach (var item in shopAction.itemsToBuy)
        {
            int ownedQuantity = inventory.GetItemQuantity(item);
            AddItemButton(new InventoryItem(item, ownedQuantity));
        }
    }

    private void BuildSellList(InventoryManager inventory)
    {
        // TODO: 已装备物品无法卖出的逻辑判断

        foreach (var item in inventory.CurrentInventory)
        {
            AddItemButton(new InventoryItem(item.ItemDefinition, item.Quantity));
        }
    }

    private void AddItemButton(InventoryItem inventoryItem, bool interactable = true, bool equippedNameFormat = false)
    {
        ItemButton itemButton = Instantiate(itemButtonPrefeb, itemButtonParent);

        if (itemButton is ShopItemButton shopItemButton)
        {
            shopItemButton.SetupButton(inventoryItem, _currentPanelType, _onItemClick);
        }
        else
        {
            itemButton.SetupButton(inventoryItem, _onItemClick);
        }

        itemButton.CurrentButton.interactable = interactable;
        itemButtons.Add(itemButton);
    }
}
