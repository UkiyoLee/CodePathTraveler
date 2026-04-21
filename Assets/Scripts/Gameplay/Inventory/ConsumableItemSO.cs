[CreateAssetMenu(menuName = "Inventory/Consumable Item")]
public class ConsumableItemSO : ItemDefinitionSO
{
    [Header("Consumable Effect")]
    [Min(0)] public int restoreAmount;
}
