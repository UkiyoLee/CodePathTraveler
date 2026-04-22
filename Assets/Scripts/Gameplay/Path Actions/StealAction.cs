public class StealAction : ActionBase
{
    [Header("Steal Action")]
    public List<InventoryItem> itemToSteal;

    public override void TriggerAction(AllyDefinitionSO interactor)
    {
        EventBus.Publish(new PanelRequestEvent(this));
    }
}