public abstract class CharacterDefinitionSO : ScriptableObject
{
    [Header("Identity")]
    public string characterID;
    public string characterName;
    public Sprite Portrait;

    [Header("Stats")]
    public int BaseLevel = 1; //角色初始等级
    public StatBlock BaseStats;
}

[System.Serializable]
/// <summary>
/// 角色属性结构体，用于存储角色的各项基础属性
/// </summary>
public struct StatBlock
{
    /// <summary>
    /// 最大生命值
    /// </summary>
    public int MaxHP;
    /// <summary>
    /// 最大法力值
    /// </summary>
    public int MaxSP;
    /// <summary>
    /// 物理攻击力
    /// </summary>
    public int PAttack;
    /// <summary>
    /// 物理防御力
    /// </summary>
    public int PDefense;
    /// <summary>
    /// 魔法攻击力
    /// </summary>
    public int MAttack;
    /// <summary>
    /// 魔法防御力
    /// </summary>
    public int MDefense;
    /// <summary>
    /// 速度值
    /// </summary>
    public int Speed;
    /// <summary>
    /// 命中率
    /// </summary>
    public int Accuracy;
    /// <summary>
    /// 闪避率
    /// </summary>
    public int Evasion;

    public static StatBlock Zero => new();
    public static StatBlock One => new()
    {
        MaxHP = 1,
        MaxSP = 1,
        PAttack = 1,
        PDefense = 1,
        MAttack = 1,
        MDefense = 1,
        Speed = 1,
        Accuracy = 1,
        Evasion = 1,
    };

    public static StatBlock operator +(StatBlock a, StatBlock b)
    {
        return new StatBlock
        {
            MaxHP = a.MaxHP + b.MaxHP,
            MaxSP = a.MaxSP + b.MaxSP,
            PAttack = a.PAttack + b.PAttack,
            PDefense = a.PDefense + b.PDefense,
            MAttack = a.MAttack + b.MAttack,
            MDefense = a.MDefense + b.MDefense,
            Speed = a.Speed + b.Speed,
            Accuracy = a.Accuracy + b.Accuracy,
            Evasion = a.Evasion + b.Evasion,
        };
    }
}