public class ShopAction : ActionBase
{
    [Header("Shop Action")]
    public List<ItemDefinitionSO> itemsToBuy;

    public override void TriggerAction(AllyDefinitionSO interactor)
    {
        EventBus.Publish(new PanelRequestEvent(this));
    }

    public bool TryExecuteTransaction(PanelType panelType, ItemDefinitionSO itemDefinition)
    {
        var inventory = InventoryManager.Instance;

        if (panelType == PanelType.Buy)
        {
            if (!inventory.TrySpendCurrency(itemDefinition.buyPrice))
            {
                return false;
            }
            inventory.AddItem(itemDefinition, 1);
        }

        if (panelType == PanelType.Sell)
        {
            if (inventory.GetItemQuantity(itemDefinition) <= 0)
            {
                return false;
            }

            inventory.RemoveItem(itemDefinition, 1);
            inventory.AddCurrency(itemDefinition.sellPrice);
            return true;
        }

        return false;
    }
}
