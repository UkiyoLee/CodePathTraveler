public enum GameMode
{
    Explore,
    InteractionMenu,
    Battle,
    Pause
}

public enum ActiveMap
{
    Player,
    UI,
    Battle,
    None
}

public enum CameraView
{
    Explore,
    Battle,
    BattleResult
}

public enum PlayerJob
{
    Any,

    // ===== 基础职业（8个） =====
    Warrior,        // 剑士
    Apothecary,     // 药师
    Cleric,         // 神官
    Dancer,         // 舞娘
    Hunter,         // 猎人
    Merchant,       // 商人
    Scholar,        // 学者
    Thief,          // 盗贼
}

public enum GrowthRank
{
    S, A, B, C, D
}