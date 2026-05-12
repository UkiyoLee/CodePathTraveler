[CreateAssetMenu(menuName = "Inventory/Equipment Item")]
public class EquipmentItemSO : ItemDefinitionSO
{
    [Header("Equipment Config")]
    public EquipmentCategory category = EquipmentCategory.Weapon;
    public WeaponType weaponType = WeaponType.Sword;

    [Header("State Bonus")]
    public StatBlock stateBlock = StatBlock.Zero;

    public void OnValidate()
    {
        if (category != EquipmentCategory.Weapon)
        {
            weaponType = WeaponType.None;
        }
    }
}
