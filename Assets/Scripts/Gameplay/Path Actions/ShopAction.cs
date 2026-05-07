public class ShopAction : ActionBase
{
    [Header("Shop Action")]
    public List<InventoryItem> itemsToBuy;

    public override void TriggerAction(AllyDefinitionSO interactor)
    {
        EventBus.Publish(new PanelRequestEvent(this));
    }
}
