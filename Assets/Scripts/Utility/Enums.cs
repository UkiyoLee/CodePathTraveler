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

public enum ItemType
{
    Equipment = 0,
    Consumable = 1
}

public enum ItemIconKey
{
    // 物品类型枚举
    // 定义了游戏中各种物品的分类
    Weapon,         // 武器
    Armor,          // 防具
    Accessory,      // 饰品
    Healing,        // 治疗
    SP,             // SP恢复
    Revive,         // 复活
    Cure,           // 解除异常状态
    KeyItem         // 关键物品/任务物品
}

public enum PanelType
{
    Item,
    Sell,
    Buy,
    Equipment
}
