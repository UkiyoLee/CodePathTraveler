using System;

[Serializable]
public class CharacterRuntimeData
{
    public CharacterDefinitionSO Definition;

    public int Level;
    public int CurrentHP;
    public int CurrentSP;
    public int CurrentBP;
    public int CurrentEXP;

    public string DisplayName => Definition != null ? Definition.characterName : "Unknown";

    public StatBlock EquipmentStats;

    public CharacterRuntimeData(CharacterDefinitionSO definition)
    {
        Definition = definition;
        EquipmentStats = StatBlock.Zero;

        var stats = GetTotalStats();
        CurrentHP = stats.MaxHP;
        CurrentSP = stats.MaxSP;
        CurrentBP = 0;
    }

    public StatBlock GetBaseStats()
    {
        if (Definition is AllyDefinitionSO allyDefinition)
        {
            return allyDefinition.GetStatForLevel(Level);
        }
        if (Definition is EnemyDefinitionSO enemyDefinition)
        {
            return enemyDefinition.BaseStats;
        }
        return Definition != null ? Definition.BaseStats : StatBlock.Zero;
    }

    public StatBlock GetTotalStats() => GetBaseStats() + EquipmentStats;

    # region 数据变化窗口

    public void HealFull()
    {
        CurrentHP = GetTotalStats().MaxHP;
        CurrentSP = GetTotalStats().MaxSP;
    }

    public void ModifyHP(int amount)
    {
        CurrentHP += amount;
        CurrentHP = Mathf.Clamp(CurrentHP, 0, GetTotalStats().MaxHP);
    }

    public void ModifySP(int amount)
    {
        CurrentSP += amount;
        CurrentSP = Mathf.Clamp(CurrentSP, 0, GetTotalStats().MaxSP);
    }

    # endregion

}
