[CreateAssetMenu(menuName = "Character/Ally")]
public class AllyDefinitionSO : CharacterDefinitionSO
{
    [Header("Ally Specific")]
    public PlayerJob Job;

    [Header("Growth Settings")]
    public GlobalGrowthConfigSO globalGrowthConfigSO;
    public GrowthProfile growthProfile;

    # region 属性相关
    public StatBlock GetStatForLevel(int level)
    {
        float hpMult = globalGrowthConfigSO.GetCurveByRank(growthProfile.HP).Evaluate(level);
        float spMult = globalGrowthConfigSO.GetCurveByRank(growthProfile.SP).Evaluate(level);
        float pAtkMult = globalGrowthConfigSO.GetCurveByRank(growthProfile.PAttack).Evaluate(level);
        float pDefMult = globalGrowthConfigSO.GetCurveByRank(growthProfile.PDefense).Evaluate(level);
        float mAtkMult = globalGrowthConfigSO.GetCurveByRank(growthProfile.MAttack).Evaluate(level);
        float mDefMult = globalGrowthConfigSO.GetCurveByRank(growthProfile.MDefense).Evaluate(level);
        float speedMult = globalGrowthConfigSO.GetCurveByRank(growthProfile.Speed).Evaluate(level);

        return new StatBlock()
        {
            MaxHP = Mathf.RoundToInt(BaseStats.MaxHP * hpMult),
            MaxSP = Mathf.RoundToInt(BaseStats.MaxSP * spMult),
            PAttack = Mathf.RoundToInt(BaseStats.PAttack * pAtkMult),
            PDefense = Mathf.RoundToInt(BaseStats.PDefense * pDefMult),
            MAttack = Mathf.RoundToInt(BaseStats.MAttack * mAtkMult),
            MDefense = Mathf.RoundToInt(BaseStats.MDefense * mDefMult),
            Speed = Mathf.RoundToInt(BaseStats.Speed * speedMult)
        };
    }
    # endregion
}

[System.Serializable]
public struct GrowthProfile
{
    public GrowthRank HP;
    public GrowthRank SP;
    public GrowthRank PAttack;
    public GrowthRank PDefense;
    public GrowthRank MAttack;
    public GrowthRank MDefense;
    public GrowthRank Speed;
}
