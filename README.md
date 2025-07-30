# StateMachine - C# 轻量级状态机库

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)

一个轻量、高效、支持链式调用的 C# 状态机框架，适用于游戏开发、业务流程控制等需要状态管理的场景。支持嵌套状态机、事件驱动、条件转移，并提供完整的调试日志功能。

---

## 📌 特性

- ✅ **链式 API 设计**：流畅的语法，提升代码可读性与开发效率
- 🔁 **支持条件转移与事件驱动**：基于条件或事件触发状态切换
- 🧩 **支持嵌套状态机（Hierarchical FSM）**
- 🎯 **精准的调试日志系统**：可按类型过滤日志（进入、退出、更新、事件等）
- 🚀 **高性能**：基于字典查找，避免反射
- 💥 **类型安全**：使用泛型参数，避免类型转换错误
- 🧹 **资源安全释放**：提供 `Dispose()` 方法防止内存泄漏

---

## 🚀 快速开始

### 1. 定义状态枚举

```csharp
public enum GameState
{
    Idle,
    Running,
    Paused,
    GameOver
}
```

### 2. 创建状态机实例

```csharp
var stateMachine = new StateMachine<object>(null)
    .SetTag("MainGameFSM"); // 设置调试标签
```

### 3. 定义状态与转移逻辑

```csharp
stateMachine
    .State(GameState.Idle)
        .Enter(_ => Console.WriteLine("进入空闲状态"))
        .Update(_ => Console.WriteLine("空闲中..."))
        .Transition(() => Input.GetButtonDown("Start")) // 条件转移
            .To(GameState.Running)
            .Transfer(_ => Console.WriteLine("开始游戏！"))
        .End()

    .State(GameState.Running)
        .Enter(_ => Console.WriteLine("游戏运行中"))
        .Update(_ => Console.WriteLine("正在游戏中"))
        .Event("PAUSE", (_, __) => true) // 事件驱动转移
            .To(GameState.Paused)
        .Event("GAME_OVER", (_, __) => true)
            .To(GameState.GameOver)
        .End()

    .State(GameState.Paused)
        .Enter(_ => Console.WriteLine("游戏已暂停"))
        .Event("RESUME", (_, __) => true)
            .To(GameState.Running)
        .End()

    .State(GameState.GameOver)
        .Enter(_ => Console.WriteLine("游戏结束"))
        .End()

    // 设置默认起始状态
    .SetDefault(GameState.Idle)

    // 构建并初始化状态机
    .Build();
```

### 4. 更新状态机

```csharp
// 每帧调用
stateMachine.Update();
```

### 5. 发送事件

```csharp
stateMachine.Dispatch("PAUSE");
stateMachine.Dispatch("RESUME");
stateMachine.Dispatch("GAME_OVER");
```

---

## 🔗 链式 API 详解

本库核心优势是**链式调用**，所有配置一气呵成，结构清晰。

### 状态定义：`.State(TState id)`

创建一个状态节点：

```csharp
stateMachine.State(GameState.Running)
    .Enter(OnEnterRunning)
    .Update(OnUpdateRunning)
    .Exit(OnExitRunning)
    .SetTag("RunningState")
    .End();
```

> `.End()` 返回到父状态机，继续配置其他状态。

---

### 状态行为回调

| 方法 | 说明 |
|------|------|
| `.Enter(Action<TObject>)` | 进入状态时调用 |
| `.Exit(Action<TObject>)` | 退出状态时调用 |
| `.Update(Action<TObject>)` | 每帧更新（在转移判断前） |
| `.EarlyUpdate(Action<TObject>)` | 早于 Update 执行 |
| `.Initialize(Action<TObject>)` | 状态机构建时初始化 |

---

### 转移方式

#### 1. 条件转移（每帧检查）

```csharp
.Transition(param => param.Health <= 0) // 条件函数
    .To(GameState.Dead)
    .Transfer(param => Console.WriteLine("触发死亡转移"))
```

#### 2. 事件驱动转移

```csharp
.Event("JUMP", (param, data) => (int)data > 1) // 接收事件并判断
    .To(GameState.Jumping)
```

#### 3. 转移目标设置

| 方法 | 说明 |
|------|------|
| `.To<TState>(TState)` | 跳转到指定状态 |
| `.ToEntry()` | 跳回入口状态 |
| `.ToEnd()` | 跳转到退出状态（结束） |
| `.Return()` | 返回上一个状态（可用于“暂停-恢复”） |

---

### 高级转移配置

#### `Select`：为特定状态添加转移

```csharp
.Select(GameState.Running, 
        () => Input.GetKeyDown(KeyCode.P), 
        GameState.Paused)
```

#### `Any`：从任意状态触发转移（全局转移）

```csharp
.Any("QUIT", (_, __) => true, GameState.GameOver)
```

常用于“退出游戏”、“强制返回主菜单”等全局事件。

---

## 🔍 调试与日志

启用调试日志，便于开发期排查问题。

```csharp
// 设置日志输出
StateMachineDebug.Log = Debug.Log; // Unity 示例

// 设置日志过滤（支持位运算组合）
StateMachineDebug.Filter = 
    StateMachineDebug.LogFilter.OnEnter | 
    StateMachineDebug.LogFilter.OnExit |
    StateMachineDebug.LogFilter.OnEvent;
```

可用过滤类型：

- `OnInitialize`
- `OnEnter`
- `OnExit`
- `OnUpdate`
- `OnEarlyUpdate`
- `OnValidate`（条件检查）
- `OnTransfer`（转移执行）
- `OnEvent`（事件触发）
- `Everything`（全部日志）

---

## 🧱 嵌套状态机（子状态机）

支持将复杂状态拆分为子状态机：

```csharp
var playerFSM = new StateMachine<Player>(player);

var attackMachine = playerFSM.Machine(PlayerState.Attacking)
    .State(AttackState.Slash)
    .State(AttackState.Kick)
    .SetDefault(AttackState.Slash)
    .Build();

// 主状态机跳转到子状态机
playerFSM
    .State(PlayerState.Idle)
    .Transition(() => Input.Attack())
        .To(PlayerState.Attacking);
```

---

## 🧹 资源清理

使用完毕后调用 `Dispose()` 释放引用，防止内存泄漏：

```csharp
stateMachine.Dispose();
```

---

## 📦 安装方式

### 手动导入
将 `StateMachine.cs` 文件复制到你的项目中即可使用，无任何外部依赖。

---

## 🧪 示例：游戏角色状态机

```csharp
var fsm = new StateMachine<Character>(character)
    .State(CharacterState.Idle)
        .Enter(c => c.Anim("idle"))
        .Transition(() => c.MoveInput != Vector2.zero)
            .To(CharacterState.Walking)
        .End()

    .State(CharacterState.Walking)
        .Enter(c => c.Anim("walk"))
        .Transition(() => c.MoveInput == Vector2.zero)
            .To(CharacterState.Idle)
        .Event("DASH", (_, __) => true)
            .To(CharacterState.Dashing)
        .End()

    .State(CharacterState.Dashing)
        .Enter(c => c.StartDash())
        .Update(c => c.UpdateDash())
        .Transition(c => c.IsDashComplete)
            .To(CharacterState.Walking)
        .End()

    .Any("TAKE_DAMAGE", (c, _) => c.Health > 0, CharacterState.Hurt)
    .Any("DEAD", (c, _) => c.Health <= 0, CharacterState.Dead)

    .SetDefault(CharacterState.Idle)
    .Build();
```

---

## 📚 API 文档（简要）

| 类 | 说明 |
|----|------|
| `StateMachine<TObject>` | 主状态机，管理状态与转移 |
| `StateBase<TObject>` | 状态基类，定义行为回调 |
| `TransitionBase<TObject>` | 转移逻辑，包含条件、事件、目标 |
| `StateMachineDebug` | 调试工具，控制日志输出 |

---

## 📄 许可证

MIT License - 自由使用、修改与分发。

---

## 🙌 贡献

欢迎提交 Issue 与 Pull Request！  

---

> 💡 **提示**：链式 API 的核心是 `.End()` 返回上层，`.To()` 和 `.Return()` 完成转移配置。合理使用 `.SetTag()` 可提升日志可读性。
