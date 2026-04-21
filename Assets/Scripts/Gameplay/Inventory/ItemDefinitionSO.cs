public class ItemDefinitionSO : ScriptableObject
{
    public string itemName;
    [TextArea] public string itemDescription;
    public ItemType itemType;
    public ItemIconKey itemIconKey;

    public int buyPrice;
    public int sellPrice => (int)(buyPrice * 0.8f);
    public int maxStack = 99;

    [Header("稀有度")]
    public int rarityWeight = 100; // 稀有度权重
}
