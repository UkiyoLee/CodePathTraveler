using System;
using UnityEngine.AI;

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

    private bool hasAppliedInitialEquipment; // 是否已应用初始装备 

    public CharacterRuntimeData(CharacterDefinitionSO definition)
    {
        Definition = definition;
        EquipmentStats = StatBlock.Zero;

        var stats = GetTotalStats();
        CurrentHP = stats.MaxHP;
        CurrentSP = stats.MaxSP;
        CurrentBP = 0;

        ApplyInitialEquipment();
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

    public void ResetBattleBP()
    {
        CurrentBP = 0;
    }

    # endregion

    # region 装备系统

    public void ApplyInitialEquipment()
    {
        if (hasAppliedInitialEquipment) return;

        AllyDefinitionSO allyDef = Definition as AllyDefinitionSO;
        if (allyDef == null || allyDef.initialEquipment == null || allyDef.initialEquipment.Count == 0)
        {
            hasAppliedInitialEquipment = true;
            return;
        }

        if (InventoryManager.Instance is null) return;

        for (int i = 0; i < allyDef.initialEquipment.Count; i++)
        {
            var entry = allyDef.initialEquipment[i];
            var item = entry.item;

            if (item == null) continue;

            // TODO: 装备物品
            InventoryManager.Instance.AddItem(item, 1);
        }
        hasAppliedInitialEquipment = true;
    }

    # endregion

}
