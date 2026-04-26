# CodePathTraveler 项目结构文档

## 项目概述

CodePathTraveler 是一款基于 Unity 开发的 RPG 角色扮演游戏，采用类《八方旅人》风格的交互系统设计。项目采用事件驱动架构，通过 EventBus 实现模块间解耦，支持角色探索、NPC交互、队伍招募、物品管理等核心玩法。

---

## 目录结构

```
Assets/
├── Animation/           # 动画资源
│   ├── Ally/           # 盟友角色动画
│   ├── Template/       # 模板动画（战斗/场景/通用）
│   └── UI/             # UI动画
├── Arts/               # 美术资源
│   ├── 01_UI_Resources/# UI资源文件
│   ├── Characters/     # 角色美术资源
│   ├── Environment/    # 环境美术资源
│   ├── Fonts/          # 字体资源
│   ├── Icons/          # 图标资源
│   └── Mask/           # 遮罩资源
├── Game Data/          # 游戏数据（ScriptableObject）
│   ├── Character/      # 角色定义数据
│   │   └── Ally/       # 盟友角色 SO
│   ├── Growth Config/  # 成长曲线配置
│   ├── Inventory/      # 物品图标配置
│   └── Items/          # 物品定义 SO
├── Materials/          # 材质
├── Prefebs/            # 预制体
│   ├── Character/      # 角色预制体
│   │   └── Follower.prefab
│   └── UI/             # UI预制体
│       └── Field/      # 场景UI预制体
├── Scenes/             # 场景
│   ├── Persistent.unity    # 持久化场景
│   ├── Persistent/         # 场景光照数据
│   └── Town 01.unity       # 城镇场景
├── Scripts/            # 脚本代码
│   ├── Framework/      # 框架层
│   ├── Gameplay/       # 游戏逻辑层
│   ├── Player/         # 玩家控制
│   ├── UI/             # UI层
│   └── Utility/        # 工具类
├── Settings/           # 项目设置
│   ├── Input System/   # 输入系统配置
│   └── URP Settings/   # URP渲染设置
└── Terrians/           # 地形数据
```

---

## 核心架构

### 架构图

```
┌─────────────────────────────────────────────────────────────────┐
│                        EventBus (事件总线)                        │
│                     全局事件发布/订阅系统                           │
│                     Namespace: MFramework.Event                   │
└─────────────────────────────────────────────────────────────────┘
                                │
        ┌───────────────────────┼───────────────────────┐
        ▼                       ▼                       ▼
┌───────────────┐     ┌───────────────┐     ┌───────────────┐
│ GameModeManager│     │InputSystemCtrl│     │   UIManager   │
│   (游戏模式)    │     │  (输入控制)    │     │   (UI管理)    │
└───────────────┘     └───────────────┘     └───────────────┘
        │                       │                       │
        ▼                       ▼                       ▼
┌───────────────┐     ┌───────────────┐     ┌───────────────┐
│CameraModeCtrl │     │PlayerController│    │PanelController│
│   (相机控制)   │     │  (玩家控制)    │     │   (面板控制)   │
└───────────────┘     └───────────────┘     └───────────────┘
        │                       │                       │
        ▼                       ▼                       ▼
┌───────────────┐     ┌───────────────┐     ┌───────────────┐
│  PartyManager │     │PlayerInteractor│    │InteractionUIC │
│   (队伍管理)   │     │  (玩家交互)    │     │  (交互UI控制)  │
└───────────────┘     └───────────────┘     └───────────────┘
        │                       │                       │
        ▼                       ▼                       ▼
┌───────────────┐     ┌───────────────┐     ┌───────────────┐
│PartyFieldCtrl │     │InteractionBase│    │  ItemButton   │
│ (队伍跟随控制) │     │  (交互基类)    │     │  (物品按钮)   │
└───────────────┘     └───────────────┘     └───────────────┘
                              │
              ┌───────────────┼───────────────┐
              ▼               ▼               ▼
        ┌──────────┐   ┌──────────┐   ┌──────────┐
        │InquireAct│   │RecruitAct│   │StealAction│
        │ (打听)   │   │ (招募)   │   │ (偷窃)   │
        └──────────┘   └──────────┘   └──────────┘
```

---

## 模块详解

### 1. Framework 层 (`Scripts/Framework/`)

框架层提供基础设施，包含单例模式和事件系统。

#### Singleton 单例基类

**文件**: `Singleton/Singleton.cs`

```csharp
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
```

- 提供单例模式实现，自动管理唯一实例
- 继承自 MonoBehaviour，支持 Unity 生命周期
- 在 `Awake` 中自动赋值并销毁重复实例

**使用方式**:

```csharp
public class MyManager : Singleton<MyManager> { }
```

#### Event 事件系统

**文件**: `Event/EventBus.cs`, `IEvent.cs`, `IEventReceiver.cs`

事件系统采用发布-订阅模式，实现模块间解耦通信。所有事件接口定义在 `MFramework.Event` 命名空间下。

**接口定义**:

```csharp
// 事件标记接口
namespace MFramework.Event;
public interface IEvent { }

// 事件接收者接口
public interface IEventReceiver<T> where T : IEvent
{
    void OnEvent(T evt);
}
```

**EventBus 核心功能**:

- `Subscribe<TEvent>(receiver)` — 订阅事件，自动去重
- `Unsubscribe<TEvent>(receiver)` — 取消订阅
- `Publish<TEvent>(evt)` — 发布事件，自动跳过已销毁的 Unity 对象

**使用方式**:

```csharp
// 定义事件
public readonly struct MyEvent : IEvent
{
    public readonly string data;
    public MyEvent(string data) => this.data = data;
}

// 订阅事件
EventBus.Subscribe<MyEvent>(this);

// 处理事件
public void OnEvent(MyEvent evt) { /* 处理逻辑 */ }

// 发布事件
EventBus.Publish(new MyEvent("hello"));

// 取消订阅
EventBus.Unsubscribe<MyEvent>(this);
```

---

### 2. Gameplay 层 (`Scripts/Gameplay/`)

游戏逻辑层包含核心玩法系统。

#### Game Mode 游戏模式

**文件**: `Game Mode/GameModeManager.cs`, `GameModeEvents.cs`

管理游戏全局状态，控制不同游戏模式的切换。

**游戏模式枚举**:

```csharp
public enum GameMode
{
    Explore,         // 探索模式
    InteractionMenu, // 交互菜单模式
    Battle,          // 战斗模式
    Pause            // 暂停模式
}
```

**核心功能**:

- 管理当前游戏模式
- 通过 `RequestChangeGameMode()` 请求模式切换
- `CanSwitchMode()` 限制：战斗模式下禁止切换
- 模式切换时广播 `GameModeChangedEvent`

**事件**:

```csharp
public readonly struct GameModeChangedEvent : IEvent
{
    public readonly GameMode newMode;
}
```

#### Character 角色系统

**文件**: `Character/CharacterDefinitionSO.cs`, `AllyDefinitionSO.cs`, `EnemyDefinitionSO.cs`, `CharacterIdentity.cs`, `CharacterRuntimeData.cs`, `GlobalGrowthConfigSO.cs`

基于 ScriptableObject 的角色数据定义，支持盟友与敌人。

**类结构**:

```
CharacterDefinitionSO (抽象基类)
    ├── characterID       // 角色ID
    ├── characterName     // 角色名称
    ├── Portrait          // 头像
    ├── BaseLevel         // 初始等级
    ├── BaseStats         // 基础属性 (StatBlock)
    ├── fieldAnimator     // 场景动画控制器
    └── battleAnimator    // 战斗动画控制器
        │
        ├── AllyDefinitionSO (盟友定义)
        │   ├── Job               // 职业 (PlayerJob)
        │   ├── globalGrowthConfigSO  // 全局成长配置
        │   └── growthProfile     // 成长曲线配置 (GrowthProfile)
        │   └── GetStatForLevel() // 按等级计算属性
        │
        └── EnemyDefinitionSO (敌人定义)
```

**属性结构体 StatBlock**:

```csharp
public struct StatBlock
{
    public int MaxHP;      // 最大生命值
    public int MaxSP;      // 最大法力值
    public int PAttack;    // 物理攻击力
    public int PDefense;   // 物理防御力
    public int MAttack;    // 魔法攻击力
    public int MDefense;   // 魔法防御力
    public int Speed;      // 速度值
    public int Accuracy;   // 命中率
    public int Evasion;    // 闪避率
}
```

**运行时数据 CharacterRuntimeData**:

```csharp
public class CharacterRuntimeData
{
    public CharacterDefinitionSO Definition;
    public int Level;
    public int CurrentHP;
    public int CurrentSP;
    public int CurrentBP;    // Boost Points
    public int CurrentEXP;
    public StatBlock EquipmentStats;  // 装备加成

    public StatBlock GetBaseStats();   // 获取基础属性
    public StatBlock GetTotalStats();  // 基础属性 + 装备加成
    public void HealFull();
    public void ModifyHP(int amount);
    public void ModifySP(int amount);
}
```

**职业枚举**:

```csharp
public enum PlayerJob
{
    Any,        // 任意职业
    Warrior,    // 剑士
    Apothecary, // 药师
    Cleric,     // 神官
    Dancer,     // 舞娘
    Hunter,     // 猎人
    Merchant,   // 商人
    Scholar,    // 学者
    Thief,      // 盗贼
}
```

**成长等级枚举**:

```csharp
public enum GrowthRank { S, A, B, C, D }
```

**成长配置 GlobalGrowthConfigSO**:

- 提供 S/A/B/C/D 五档成长曲线（AnimationCurve）
- 根据 `GrowthProfile` 中各属性的等级计算等级倍率
- 等级范围：1 ~ 99

#### Party 队伍系统

**文件**: `Party/PartyManager.cs`, `PartyFieldController.cs`, `FieldFollower.cs`

管理队伍成员及场景中的跟随表现。

**PartyManager**:

- 单例管理器，维护 `List<CharacterRuntimeData> PartyMembers`
- 初始队伍自动加入玩家角色
- `RecruitMember()` 招募新成员，并刷新跟随者

**PartyFieldController**:

- 基于路径记录的跟随系统
- 记录玩家移动轨迹（最多30个点），按距离采样计算跟随位置
- 动态创建/销毁 `FieldFollower` 实例
- 可配置：跟随距离、移动速度、Z轴偏移

**FieldFollower**:

- 使用 `CharacterController` 移动
- 重力系统和地面吸附
- 根据移动方向更新动画参数 (`moveX`, `moveY`, `isMoving`)
- 通过 `SetupFollower()` 切换角色外观动画

#### Input 输入系统

**文件**: `Input/InputSystemController.cs`

基于 Unity Input System 的输入控制器。

**核心功能**:

- 管理输入动作映射 (Action Map)
- 根据游戏模式自动切换输入方案
- 提供输入查询接口

**输入映射**:

```csharp
public enum ActiveMap
{
    Player,  // 玩家移动控制
    UI,      // UI导航控制
    Battle,  // 战斗控制
    None     // 无输入
}
```

**关键方法**:

```csharp
Vector2 GetMovementInput();     // 获取移动输入
bool GetPlayerConfirmPressed(); // 获取确认键（仅在 Player Map 下）
bool GetUICancelPressed();      // 获取取消键（仅在 UI Map 下）
```

**模式映射规则**:

- `Explore` → `Player` Map
- `Battle`, `InteractionMenu`, `Pause` → `UI` Map

#### Interaction 交互系统

**文件**: `Interaction/InteractionBase.cs`, `InteractionEvents.cs`

核心交互系统，支持多种交互行为。

**事件定义**:

```csharp
// 交互状态变化事件
public readonly struct InteractionChangedEvent : IEvent
{
    public readonly InteractionBase target;
    public readonly bool inRange;  // 是否在交互范围内
}

// 交互菜单请求事件
public readonly struct InteractionMenuRequestEvent : IEvent
{
    public readonly InteractionBase target;
}
```

**核心流程**:

1. 玩家进入交互范围 → `OnTriggerEnter` → `OnFocus()` → 显示头顶图标
2. 玩家确认交互 → `Interact()` → 打开交互菜单
3. 选择动作 → `ExecuteCommandFromUI()` → 执行具体动作
4. 玩家离开范围 → `OnTriggerExit` → `OnUnfocus()` → 隐藏图标

**命令缓存与过滤**:

- `RebuildCommands()` 根据当前交互者的职业过滤可显示的动作
- 按 `ActionCommandInfo.Order` 排序
- 缓存结果供 UI 使用

#### Path Actions 动作系统

**文件**: `Path Actions/ActionBase.cs`, `InquireAction.cs`, `RecruitAction.cs`, `StealAction.cs`

可配置的交互动作系统。

**动作基类**:

```csharp
public abstract class ActionBase : MonoBehaviour
{
    public PlayerJob MatchJob = PlayerJob.Any;    // 匹配职业
    public ActionCommandInfo CommandInfo;         // 命令信息

    public virtual bool CanShow(AllyDefinitionSO interactor);     // 默认按职业匹配
    public virtual bool CanExecute(AllyDefinitionSO interactor);  // 默认可执行
    public virtual void TriggerAction(AllyDefinitionSO interactor); // 默认直接 Execute
    public virtual void Execute(object contextData = null);       // 执行逻辑
}
```

**命令信息结构**:

```csharp
public struct ActionCommandInfo
{
    public string DisplayName; // 显示名称
    public Sprite Icon;        // 图标
    public int Order;          // 排序顺序
}
```

**具体动作**:

| 动作类          | 功能     | 说明                                          |
| --------------- | -------- | --------------------------------------------- |
| `InquireAction` | 打听消息 | 从NPC获取随机消息，打开面板显示               |
| `RecruitAction` | 招募队员 | 将NPC招募到队伍，通过 `PartyManager` 添加     |
| `StealAction`   | 偷窃物品 | 显示可偷窃物品列表，通过 `InventoryItem` 配置 |

**InquireActionData**:

```csharp
public class InquireActionData
{
    public string title;           // 消息标题
    public string personName;      // 说话人
    public string message;         // 消息内容
    public Sprite portraitOverride;// 立绘覆盖
}
```

#### Inventory 物品系统

**文件**: `Inventory/InventoryManager.cs`, `ItemDefinitionSO.cs`, `ConsumableItemSO.cs`, `EquipmentItemSO.cs`, `ItemIconSetSO.cs`

基于 ScriptableObject 的物品管理与背包系统。

**物品基类 ItemDefinitionSO**:

```csharp
public class ItemDefinitionSO : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public ItemType itemType;       // Equipment / Consumable
    public ItemIconKey itemIconKey; // 图标键值
    public int buyPrice;
    public int sellPrice;           // buyPrice * 0.8f
    public int maxStack = 99;
    public int rarityWeight = 100;  // 稀有度权重
}
```

**物品类型枚举**:

```csharp
public enum ItemType
{
    Equipment = 0,   // 装备
    Consumable = 1   // 消耗品
}

public enum ItemIconKey
{
    Weapon, Armor, Accessory,  // 装备类
    Healing, SP, Revive, Cure, // 消耗品类
    KeyItem                    // 关键物品
}
```

**ConsumableItemSO**:

- 继承自 `ItemDefinitionSO`
- `restoreAmount` — 恢复量

**InventoryManager**:

- 单例管理器
- `CurrentInventory: List<InventoryItem>` — 当前背包
- `IconSet: ItemIconSetSO` — 图标映射配置
- `AddItem()` / `RemoveItem()` / `GetItemQuantity()`

**InventoryItem**:

```csharp
public class InventoryItem
{
    public ItemDefinitionSO ItemDefinition;
    public int Quantity;
    public bool IsEquipment;
    public bool IsConsumable;
}
```

**ItemIconSetSO**:

- 通过 `ItemIconKey` 查询对应图标 Sprite
- 用于 UI 中动态显示物品图标

#### Camera 相机系统

**文件**: `Camera/CameraModeController.cs`

根据游戏模式切换相机视角。

**相机视角**:

```csharp
public enum CameraView
{
    Explore,        // 探索视角（跟随相机）
    Battle,         // 战斗视角（战斗相机）
    BattleResult    // 战斗结果视角（预留）
}
```

**当前实现**:

- `Explore` → 激活 `followCamera`
- `Battle` → 激活 `battleCamera`

---

### 3. Player 层 (`Scripts/Player/`)

玩家控制相关逻辑。

#### PlayerController 玩家控制器

**文件**: `PlayerController.cs`

控制角色移动和动画。

**核心功能**:

- 使用 `CharacterController` 实现角色移动
- 重力系统和地面吸附 (`groundSnapForce`)
- 移动动画控制 (方向 `moveX`/`moveY`，速度 `isMoving`)

**关键属性**:

```csharp
[SerializeField] private float speed = 5.0f;        // 移动速度
[SerializeField] private float gravity = -9.81f;    // 重力
[SerializeField] private float groundSnapForce = -2f; // 地面吸附力
[SerializeField] private float maxGravity = -30f;    // 最大下落速度
```

#### PlayerInteractor 玩家交互器

**文件**: `PlayerInteractor.cs`

处理玩家与交互对象的交互。

**工作流程**:

1. `OnTriggerEnter` → 检测可交互对象 → 调用 `OnFocus()`
2. 玩家按下确认键 → 调用 `Interact()`
3. `OnTriggerExit` → 调用 `OnUnfocus()`

**依赖**:

- 需要同对象上有 `CharacterIdentity` 组件来获取当前角色定义
- 将 `CharacterDefinitionSO` 转换为 `AllyDefinitionSO` 传给交互系统

---

### 4. UI 层 (`Scripts/UI/`)

用户界面系统。

#### UIManager UI管理器

**文件**: `UIManager.cs`

管理所有UI面板的显示和隐藏。

**核心功能**:

- 自动收集所有 `PanelController`
- 响应 `PanelRequestEvent` 打开对应面板
- 处理取消输入关闭面板
- 交互菜单模式下，优先关闭当前打开的面板

**事件响应**:

```csharp
public void OnEvent(PanelRequestEvent evt)
{
    var panelType = evt.actionBase.GetType();
    _panelControllerDict.TryGetValue(panelType, out var panel);
    panel?.gameObject.SetActive(true);
    panel?.SetupPanel(evt.actionBase);
}
```

#### PanelController 面板控制器基类

**文件**: `Field Panel/PanelController.cs`

所有UI面板的基类。支持 DOTween 动画。

**核心属性**:

```csharp
public ActionBase CurrentAction;      // 当前动作
public Button FirstButton;            // 默认选中按钮
public virtual Type PanelActionType;  // 关联的动作类型
```

**核心方法**:

```csharp
public virtual void SetupPanel(ActionBase actionBase); // 初始化面板
public virtual void ClosePanel();                      // 关闭面板（支持 DOTween 倒放）
protected void OnConfirm();                            // 确认：执行动作并关闭
protected void OnCancel();                             // 取消：切回探索模式并关闭
protected void SetDefaultSelection();                  // 设置默认选中按钮
protected void RebindButtons(Button, UnityAction);     // 重新绑定按钮事件
```

#### InteractionUIController 交互UI控制器

**文件**: `Field Panel/InteractionUIController.cs`

管理交互相关的UI显示，包括头顶图标和交互菜单。

**核心功能**:

- 使用 `ObjectPool<GameObject>` 管理UI元素（头顶图标、菜单按钮）
- 显示NPC头顶的动作图标
- 显示交互菜单按钮列表
- 跟踪NPC位置更新UI位置（`LateUpdate` 中世界坐标转屏幕坐标）
- 处理取消键关闭菜单

**对象池配置**:

```csharp
_iconPool = new ObjectPool<GameObject>(
    createFunc: () => Instantiate(actionIconPrefeb, actionIconHolder),
    defaultCapacity: 8,
    maxSize: 20
);
```

#### 具体面板控制器

| 控制器                   | 关联动作        | 功能                                       |
| ------------------------ | --------------- | ------------------------------------------ |
| `InquirePanelController` | `InquireAction` | 显示打听到的消息（名称、标题、内容、头像） |
| `RecruitPanelController` | `RecruitAction` | 显示招募确认界面（名称、等级、立绘）       |
| `StealPanelController`   | `StealAction`   | 显示可偷窃物品列表，含确认弹窗             |

#### 物品相关UI组件

**文件**: `UI/Inventory/ItemButton.cs`, `StealItemIcon.cs`

**ItemButton**:

- 继承 `ISelectHandler`, `IDeselectHandler`
- 显示物品图标、名称、描述、数量
- 选中时显示 `itemTips`
- 通过 `InventoryManager.Instance.IconSet.GetIcon()` 获取图标

**StealItemIcon**:

- 继承自 `ItemButton`
- 额外显示偷窃成功率 (`rarityWeight%`)

#### 辅助组件

| 组件               | 文件                                  | 功能                                                                   |
| ------------------ | ------------------------------------- | ---------------------------------------------------------------------- |
| `ActionMenuButton` | `Field Panel/ActionMenuButton.cs`     | 交互菜单按钮，显示图标和文字                                           |
| `LayoutMaxWidth`   | `Field Panel/ChatBubbleController.cs` | 文本最大宽度限制，自动换行（基于 `LayoutElement` + `TextMeshProUGUI`） |

---

### 5. Utility 层 (`Scripts/Utility/`)

工具类和通用定义。

**文件列表**:

- `Enums.cs` — 所有枚举定义（`GameMode`, `ActiveMap`, `CameraView`, `PlayerJob`, `GrowthRank`, `ItemType`, `ItemIconKey`）
- `GlobalUsing.cs` — 全局 using 语句（`System.Collections`, `System.Collections.Generic`, `UnityEngine`）

---

## 事件流图

### 交互流程事件流

```
玩家进入NPC范围
       │
       ▼
PlayerInteractor.OnTriggerEnter()
       │
       ▼
InteractionBase.OnFocus()
       │
       ▼
EventBus.Publish(InteractionChangedEvent)
       │
       ▼
InteractionUIController.OnEvent()
       │
       ▼
显示头顶动作图标
       │
       ▼
玩家按下确认键
       │
       ▼
InteractionBase.Interact()
       │
       ▼
EventBus.Publish(InteractionMenuRequestEvent)
       │
       ▼
InteractionUIController.OnEvent()
       │
       ▼
GameModeManager.RequestChangeGameMode(InteractionMenu)
       │
       ▼
EventBus.Publish(GameModeChangedEvent)
       │
       ▼
InputSystemController 切换输入映射 (Player → UI)
       │
       ▼
显示交互菜单
       │
       ▼
玩家选择动作
       │
       ▼
InteractionBase.ExecuteCommandFromUI()
       │
       ▼
ActionBase.TriggerAction()
       │
       ▼
EventBus.Publish(PanelRequestEvent)
       │
       ▼
UIManager.OnEvent()
       │
       ▼
显示对应面板（Inquire/Recruit/Steal）
```

### 队伍招募事件流

```
RecruitPanelController 确认招募
       │
       ▼
RecruitAction.Execute()
       │
       ▼
PartyManager.RecruitMember()
       │
       ├─ 添加 CharacterRuntimeData 到 PartyMembers
       ├─ PartyFieldController.UpdateFollowers()
       │   ├─ 实例化 FieldFollower 预制体
       │   └─ 设置角色动画外观
       └─ GameModeManager.RequestChangeGameMode(Explore)
```

---

## 扩展指南

### 添加新的交互动作

1. **创建动作类**:

```csharp
public class MyAction : ActionBase
{
    public override void TriggerAction(AllyDefinitionSO interactor)
    {
        EventBus.Publish(new PanelRequestEvent(this));
    }
}
```

2. **创建面板控制器**:

```csharp
public class MyPanelController : PanelController
{
    public override Type PanelActionType => typeof(MyAction);

    public override void SetupPanel(ActionBase actionBase)
    {
        base.SetupPanel(actionBase);
        // 初始化面板显示
        SetDefaultSelection();
    }
}
```

3. 将面板预制体挂载到 `UIManager` 下，确保 `PanelController` 组件被自动收集。

### 添加新的游戏模式

1. 在 `Enums.cs` 添加新模式:

```csharp
public enum GameMode
{
    Explore,
    InteractionMenu,
    Battle,
    Pause,
    MyNewMode  // 新模式
}
```

2. 在 `InputSystemController.GetMapFromGameMode()` 配置输入映射

3. 订阅 `GameModeChangedEvent` 处理模式切换

### 添加新的物品类型

1. 创建继承自 `ItemDefinitionSO` 的 ScriptableObject:

```csharp
[CreateAssetMenu(menuName = "Inventory/My Item")]
public class MyItemSO : ItemDefinitionSO
{
    // 自定义属性
}
```

2. 在 `ItemIconKey` 枚举中添加新图标键值

3. 在 `ItemIconSetSO` 中配置对应图标

---

## 技术栈

- **游戏引擎**: Unity 2022.3+
- **渲染管线**: URP (Universal Render Pipeline)
- **输入系统**: Unity Input System
- **UI框架**: UGUI + TextMesh Pro
- **动画插件**: DOTween
- **代码风格**: C# 10+ (file-scoped namespaces, global using)

---

## 命名规范

- **类名**: PascalCase (如 `PlayerController`)
- **方法名**: PascalCase (如 `GetMovementInput`)
- **私有字段**: `_camelCase` (如 `_currentInteractor`)
- **公共属性**: PascalCase (如 `Instance`)
- **事件**: 以 `Event` 结尾 (如 `GameModeChangedEvent`)
- **接口**: 以 `I` 开头 (如 `IEvent`)
- **命名空间**: `MFramework.Event`

---

## 文件引用速查

| 功能       | 关键文件                                              |
| ---------- | ----------------------------------------------------- |
| 单例模式   | `Scripts/Framework/Singleton/Singleton.cs`            |
| 事件总线   | `Scripts/Framework/Event/EventBus.cs`                 |
| 游戏模式   | `Scripts/Gameplay/Game Mode/GameModeManager.cs`       |
| 输入控制   | `Scripts/Gameplay/Input/InputSystemController.cs`     |
| 交互系统   | `Scripts/Gameplay/Interaction/InteractionBase.cs`     |
| 动作基类   | `Scripts/Gameplay/Path Actions/ActionBase.cs`         |
| 角色定义   | `Scripts/Gameplay/Character/CharacterDefinitionSO.cs` |
| 盟友定义   | `Scripts/Gameplay/Character/AllyDefinitionSO.cs`      |
| 运行时数据 | `Scripts/Gameplay/Character/CharacterRuntimeData.cs`  |
| 成长配置   | `Scripts/Gameplay/Character/GlobalGrowthConfigSO.cs`  |
| 队伍管理   | `Scripts/Gameplay/Party/PartyManager.cs`              |
| 跟随控制   | `Scripts/Gameplay/Party/PartyFieldController.cs`      |
| 背包管理   | `Scripts/Gameplay/Inventory/InventoryManager.cs`      |
| 物品定义   | `Scripts/Gameplay/Inventory/ItemDefinitionSO.cs`      |
| 玩家控制   | `Scripts/Player/PlayerController.cs`                  |
| 玩家交互   | `Scripts/Player/PlayerInteractor.cs`                  |
| UI管理     | `Scripts/UI/UIManager.cs`                             |
| 面板基类   | `Scripts/UI/Field Panel/PanelController.cs`           |
| 交互UI控制 | `Scripts/UI/Field Panel/InteractionUIController.cs`   |
| 物品按钮   | `Scripts/UI/Inventory/ItemButton.cs`                  |
| 枚举定义   | `Scripts/Utility/Enums.cs`                            |
| 全局引用   | `Scripts/Utility/GlobalUsing.cs`                      |
