public class StealAction : ActionBase
{
    [Header("Steal Action")]
    public List<InventoryItem> itemToSteal;

    public override void TriggerAction(AllyDefinitionSO interactor)
    {
        EventBus.Publish(new PanelRequestEvent(this));
    }

    public bool TrySteal(ItemDefinitionSO itemDefinition)
    {
        if (itemDefinition == null)
        {
            return false;
        }
        
        bool success = Random.value <= Mathf.Clamp01(itemDefinition.rarityWeight / 100f);

        if (success)
        {
            InventoryManager.Instance.AddItem(itemDefinition, 1);

            for (int i = 0; i < itemToSteal.Count; i++)
            {
                if (itemToSteal[i].ItemDefinition != itemDefinition)
                {
                    continue;
                }
                itemToSteal.RemoveAt(i);
            }
        }

        return success;
    }
}