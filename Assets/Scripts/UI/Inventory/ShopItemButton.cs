using System;
using TMPro;

public class ShopItemButton : ItemButton
{
    [Header("Shop Item Button")]
    [SerializeField] private TMP_Text priceText;

    public void SetupButton(InventoryItem item, PanelType panelType, Action<ItemDefinitionSO> onItemClick)
    {
        base.SetupButton(item, onItemClick);
        priceText.text = panelType == PanelType.Buy
        ? $"￥{item.ItemDefinition.buyPrice}"
        : $"￥{item.ItemDefinition.sellPrice}";
    }
}
