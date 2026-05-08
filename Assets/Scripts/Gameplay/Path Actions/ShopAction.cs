public class ShopAction : ActionBase
{
    [Header("Shop Action")]
    public List<ItemDefinitionSO> itemsToBuy;

    public override void TriggerAction(AllyDefinitionSO interactor)
    {
        EventBus.Publish(new PanelRequestEvent(this));
    }
}
