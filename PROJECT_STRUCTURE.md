# CodePathTraveler 项目结构文档

## 项目概述

CodePathTraveler 是一款基于 Unity 开发的 RPG 角色扮演游戏，采用类《八方旅人》风格的交互系统设计。项目采用事件驱动架构，通过 EventBus 实现模块间解耦，支持角色探索、NPC交互、队伍招募等核心玩法。

---

## 目录结构

```
Assets/
├── Animation/           # 动画资源
│   ├── Ally/           # 盟友角色动画
│   ├── Template/       # 模板动画
│   └── UI/             # UI动画
├── Arts/               # 美术资源
│   ├── 01_UI_Resources/   # UI资源文件
│   ├── Characters/     # 角色美术资源
│   ├── Environment/    # 环境美术资源
│   ├── Fonts/          # 字体资源
│   ├── Icons/          # 图标资源
│   └── Mask/           # 遮罩资源
├── Game Data/          # 游戏数据
│   └── Character/      # 角色定义数据
│       └── Ally/       # 盟友角色 ScriptableObject
├── Materials/          # 材质
├── Prefebs/            # 预制体
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
                              │
                              ▼
                    ┌───────────────┐
                    │PlayerInteractor│
                    │  (玩家交互)    │
                    └───────────────┘
                              │
                              ▼
                    ┌───────────────┐
                    │InteractionBase│
                    │  (交互基类)    │
                    └───────────────┘
                              │
              ┌───────────────┼───────────────┐
              ▼               ▼               ▼
        ┌──────────┐   ┌──────────┐   ┌──────────┐
        │InquireAct│   │RecruitAct│   │  ...     │
        │ (打听)   │   │ (招募)   │   │          │
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

- 提供线程安全的单例模式实现
- 继承自 MonoBehaviour，支持 Unity 生命周期
- 自动销毁重复实例

**使用方式**:
```csharp
public class MyManager : Singleton<MyManager> { }
```

#### Event 事件系统

**文件**: `Event/EventBus.cs`, `IEvent.cs`, `IEventReceiver.cs`

事件系统采用发布-订阅模式，实现模块间解耦通信。

**接口定义**:
```csharp
// 事件标记接口
public interface IEvent { }

// 事件接收者接口
public interface IEventReceiver<T> where T : IEvent
{
    void OnEvent(T evt);
}
```

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
- 通过事件通知模式切换
- 战斗模式下禁止切换

**事件**:
```csharp
public readonly struct GameModeChangedEvent : IEvent
{
    public readonly GameMode newMode;
}
```

#### Character 角色系统

**文件**: `Character/CharacterDefinitionSO.cs`, `CharacterIdentity.cs`, `AllyDefinitionSO.cs`

基于 ScriptableObject 的角色数据定义。

**类结构**:
```
CharacterDefinitionSO (抽象基类)
    ├── characterID    // 角色ID
    ├── characterName  // 角色名称
    └── Portrait       // 头像
        │
        └── AllyDefinitionSO (盟友定义)
            └── Job    // 职业
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
bool GetPlayerConfirmPressed(); // 获取确认键
bool GetUICancelPressed();      // 获取取消键
```

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
1. 玩家进入交互范围 → `OnFocus()` → 显示头顶图标
2. 玩家确认交互 → `Interact()` → 打开交互菜单
3. 选择动作 → `ExecuteCommandFromUI()` → 执行具体动作
4. 玩家离开范围 → `OnUnfocus()` → 隐藏图标

#### Path Actions 动作系统

**文件**: `Path Actions/ActionBase.cs`, `InquireAction.cs`, `RecruitAction.cs`

可配置的交互动作系统。

**动作基类**:
```csharp
public abstract class ActionBase : MonoBehaviour
{
    public PlayerJob MatchJob;        // 匹配职业
    public ActionCommandInfo CommandInfo; // 命令信息
    
    public virtual bool CanShow(AllyDefinitionSO interactor);    // 是否显示
    public virtual bool CanExecute(AllyDefinitionSO interactor); // 是否可执行
    public virtual void TriggerAction(AllyDefinitionSO interactor); // 触发动作
    public virtual void Execute(object contextData = null);      // 执行逻辑
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

| 动作类 | 功能 | 说明 |
|--------|------|------|
| `InquireAction` | 打听消息 | 从NPC获取随机消息 |
| `RecruitAction` | 招募队员 | 将NPC招募到队伍 |

#### Camera 相机系统

**文件**: `Camera/CameraModeController.cs`

根据游戏模式切换相机视角。

**相机视角**:
```csharp
public enum CameraView
{
    Explore,        // 探索视角
    Battle,         // 战斗视角
    BattleResult    // 战斗结果视角
}
```

---

### 3. Player 层 (`Scripts/Player/`)

玩家控制相关逻辑。

#### PlayerController 玩家控制器

**文件**: `PlayerController.cs`

控制角色移动和动画。

**核心功能**:
- 使用 CharacterController 实现角色移动
- 重力系统和地面吸附
- 移动动画控制 (方向、速度)

**关键属性**:
```csharp
[SerializeField] private float speed = 5.0f;        // 移动速度
[SerializeField] private float gravity = -9.81f;    // 重力
[SerializeField] private float groundSnapForce = -2f; // 地面吸附力
```

#### PlayerInteractor 玩家交互器

**文件**: `PlayerInteractor.cs`

处理玩家与交互对象的交互。

**工作流程**:
1. `OnTriggerEnter` → 检测可交互对象 → 调用 `OnFocus()`
2. 玩家按下确认键 → 调用 `Interact()`
3. `OnTriggerExit` → 调用 `OnUnfocus()`

---

### 4. UI 层 (`Scripts/UI/`)

用户界面系统。

#### UIManager UI管理器

**文件**: `UIManager.cs`

管理所有UI面板的显示和隐藏。

**核心功能**:
- 自动收集所有 PanelController
- 响应面板请求事件
- 处理取消输入关闭面板

**事件响应**:
```csharp
public void OnEvent(PanelRequestEvent evt)
{
    // 根据动作类型找到对应面板并显示
    var panelType = evt.actionBase.GetType();
    _panelControllerDict.TryGetValue(panelType, out var panel);
    panel?.gameObject.SetActive(true);
    panel?.SetupPanel(evt.actionBase);
}
```

#### PanelController 面板控制器基类

**文件**: `Field Panel/PanelController.cs`

所有UI面板的基类。

**核心属性**:
```csharp
public ActionBase CurrentAction;      // 当前动作
public Button FirstButton;            // 默认选中按钮
public virtual Type PanelActionType;  // 关联的动作类型
```

**核心方法**:
```csharp
public virtual void SetupPanel(ActionBase actionBase); // 初始化面板
public virtual void ClosePanel();                      // 关闭面板
protected void SetDefaultSelection();                  // 设置默认选中
```

#### InteractionUIController 交互UI控制器

**文件**: `Field Panel/InteractionUIController.cs`

管理交互相关的UI显示，包括头顶图标和交互菜单。

**核心功能**:
- 使用对象池管理UI元素
- 显示NPC头顶的动作图标
- 显示交互菜单按钮列表
- 跟踪NPC位置更新UI位置

**对象池配置**:
```csharp
_iconPool = new ObjectPool<GameObject>(
    createFunc: () => Instantiate(actionIconPrefeb, actionIconHolder),
    defaultCapacity: 8,
    maxSize: 20
);
```

#### 具体面板控制器

| 控制器 | 关联动作 | 功能 |
|--------|----------|------|
| `InquirePanelController` | `InquireAction` | 显示打听到的消息 |
| `RecruitPanelController` | `RecruitAction` | 显示招募确认界面 |

#### 辅助组件

| 组件 | 功能 |
|------|------|
| `ActionMenuButton` | 交互菜单按钮，显示图标和文字 |
| `LayoutMaxWidth` | 文本最大宽度限制，自动换行 |

---

### 5. Utility 层 (`Scripts/Utility/`)

工具类和通用定义。

**文件列表**:
- `Enums.cs` - 所有枚举定义
- `GlobalUsing.cs` - 全局 using 语句

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
InputSystemController 切换输入映射
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
显示对应面板
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
    }
}
```

3. **创建面板请求事件** (如需要):
```csharp
// PanelRequestEvent 已通用化，直接使用即可
```

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

---

## 技术栈

- **游戏引擎**: Unity 2022.3+
- **渲染管线**: URP (Universal Render Pipeline)
- **输入系统**: Unity Input System
- **UI框架**: UGUI + TextMesh Pro
- **代码风格**: C# 10+ (file-scoped namespaces, global using)

---

## 命名规范

- **类名**: PascalCase (如 `PlayerController`)
- **方法名**: PascalCase (如 `GetMovementInput`)
- **私有字段**: `_camelCase` (如 `_currentInteractor`)
- **公共属性**: PascalCase (如 `Instance`)
- **事件**: 以 `Event` 结尾 (如 `GameModeChangedEvent`)
- **接口**: 以 `I` 开头 (如 `IEvent`)

---

## 文件引用速查

| 功能 | 关键文件 |
|------|----------|
| 单例模式 | `Scripts/Framework/Singleton/Singleton.cs` |
| 事件总线 | `Scripts/Framework/Event/EventBus.cs` |
| 游戏模式 | `Scripts/Gameplay/Game Mode/GameModeManager.cs` |
| 输入控制 | `Scripts/Gameplay/Input/InputSystemController.cs` |
| 交互系统 | `Scripts/Gameplay/Interaction/InteractionBase.cs` |
| 动作基类 | `Scripts/Gameplay/Path Actions/ActionBase.cs` |
| 玩家控制 | `Scripts/Player/PlayerController.cs` |
| UI管理 | `Scripts/UI/UIManager.cs` |
| 枚举定义 | `Scripts/Utility/Enums.cs` |
