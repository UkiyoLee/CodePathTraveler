using System;
using TMPro;

public class StealItemIcon : ItemButton
{
    [Header("Steal Item Icon")]
    [SerializeField] private TMP_Text rateText;

    public override void SetupButton(InventoryItem item, Action<ItemDefinitionSO> onItemClick)
    {
        base.SetupButton(item, onItemClick);
        rateText.text = $"{item.ItemDefinition.rarityWeight}%";
    }
}
