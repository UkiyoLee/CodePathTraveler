[CreateAssetMenu(menuName = "Inventory/Item Icon Set")]
public class ItemIconSetSO : ScriptableObject
{
    public ItemIconEntry[] itemIconEntries;

    public Sprite GetIcon(ItemIconKey itemIconKey)
    {
        foreach (var entry in itemIconEntries)
        {
            if (entry.itemIconKey == itemIconKey)
            {
                return entry.icon;
            }
        }
        return null;
    }
}

[System.Serializable]
public class ItemIconEntry
{
    public ItemIconKey itemIconKey;
    public Sprite icon;
}
