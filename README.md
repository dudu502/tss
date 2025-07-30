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
        .Transition(_ => Input.GetButtonDown("Start")) // 条件转移
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
        (param) => Input.GetKeyDown(KeyCode.P), 
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
    .Transition(player => Input.Attack())
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
        .Transition(c => c.MoveInput != Vector2.zero)
            .To(CharacterState.Walking)
        .End()

    .State(CharacterState.Walking)
        .Enter(c => c.Anim("walk"))
        .Transition(c => c.MoveInput == Vector2.zero)
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

# 行为树框架 API 文档 (Readme)

本框架提供了一个轻量级、类型安全且易于使用的 C# 行为树（Behavior Tree）实现。其核心特点是**流畅的链式 API（Fluent API）**，允许您以声明式的方式快速构建复杂的行为逻辑。

---

## 核心概念

*   **`BehaviourTree<T>`**: 行为树的根对象。`T` 是传递给树中所有节点的上下文或参数类型（例如 `Player`、`AICharacter`）。
*   **`Node<T>`**: 所有节点的抽象基类。它定义了节点的执行生命周期（`OnStart`, `OnUpdate`, `OnStop`）和结果（`NodeResult`）。
*   **`NodeResult`**: 节点执行后的返回结果。
    *   `Continue`: 节点仍在执行中，需要在下一帧继续。
    *   `Success`: 节点成功完成。
    *   `Failure`: 节点执行失败。
*   **链式 API**: 所有构建方法都返回 `this` 或 `BehaviourTree<T>` 实例，允许您通过连续的调用（点号连接）来构建整棵树。

---

## 快速开始

1.  **创建行为树实例**:
    使用您的上下文类型 `T` 创建 `BehaviourTree<T>`。

    ```csharp
    // 假设有一个名为 AIContext 的类作为上下文
    var aiContext = new AIContext();
    var behaviourTree = new BehaviourTree<AIContext>(aiContext);
    ```

2.  **构建行为树**:
    使用链式 API 从 `behaviourTree` 开始，像搭积木一样添加节点。

3.  **执行行为树**:
    在游戏循环或更新方法中调用 `Execute()`。

    ```csharp
    void Update()
    {
        behaviourTree.Execute();
    }
    ```

---

## 链式 API 详解

### 1. 控制节点 (Composite Nodes)

这些节点用于组织和控制子节点的执行流程。

*   **`Sequence(string tag = null)`**: **顺序节点**。按添加顺序执行子节点。如果任一子节点返回 `Failure`，则立即返回 `Failure`。如果所有子节点都返回 `Success`，则返回 `Success`。

    ```csharp
    behaviourTree
        .Sequence() // 开始一个顺序节点
            .Do("Check Health", context => context.Health < 50 ? NodeResult.Success : NodeResult.Failure)
            .WaitTime(1000) // 等待1秒
            .Do("Heal", context => { context.Health += 10; return NodeResult.Success; })
        .End(); // 结束当前节点（Sequence），回到上一级
    ```

*   **`Select(string tag = null)`**: **选择节点 (Selector)**。按添加顺序执行子节点。如果任一子节点返回 `Success`，则立即返回 `Success`。如果所有子节点都返回 `Failure`，则返回 `Failure`。

    ```csharp
    behaviourTree
        .Select() // 开始一个选择节点
            .Do("Attack", TryAttack)
            .Do("Flee", TryFlee)
            .Do("Hide", TryHide)
        .End();
    ```

*   **`Parallel(string tag = null)`**: **并行节点**。在**同一帧**内并行执行所有子节点。最终结果：
    *   如果**任一**子节点返回 `Failure`，则结果为 `Failure`。
    *   如果所有子节点都返回 `Success`，则结果为 `Success`。
    *   否则（有 `Continue` 或混合状态），结果为 `Continue`。

    ```csharp
    behaviourTree
        .Parallel()
            .Do("Move To Target", MoveToTarget)
            .Do("Play Animation", PlayAttackAnimation)
        .End();
    ```

*   **`ParallelSelect(string tag = null)`**: **并行选择节点**。并行执行所有子节点，但**一旦有任意一个子节点返回 `Success` 或 `Failure`，就立即停止执行其他子节点并返回该结果**。如果所有子节点都返回 `Failure`，则返回 `Failure`。

*   **`ParallelSequence(string tag = null)`**: **并行顺序节点**。并行执行所有子节点，但**只有当所有子节点都返回 `Success` 时，才返回 `Success`**。如果任一子节点返回 `Failure`，则立即返回 `Failure`。

---

### 2. 装饰节点 (Decorator Nodes)

这些节点修改或控制其单个子节点的行为。

*   **`Invert(string tag = null)`**: **取反节点**。执行其子节点，然后将结果取反（`Success` 变 `Failure`，`Failure` 变 `Success`）。

    ```csharp
    behaviourTree
        .Invert()
            .Do("Is Enemy Close", context => context.EnemyDistance < 5 ? NodeResult.Success : NodeResult.Failure)
        .End(); // 如果敌人靠近则返回 Failure，否则返回 Success
    ```

*   **`Repeat(string tag = null)`**: **重复节点**。无限循环执行其子节点，结果始终为 `Continue`。

*   **`RepeatUntil(Func<T, bool> repeatUntil, string tag = null)`**: **直到...为止重复节点**。重复执行其子节点，直到提供的 `repeatUntil` 函数返回 `true`，然后返回 `Success`。

    ```csharp
    behaviourTree
        .RepeatUntil(context => context.IsTaskComplete) // 重复直到任务完成
            .Do("Work", DoWork)
        .End();
    ```

*   **`Success(string tag = null)`**: **强制成功节点**。执行其子节点，然后**强制返回 `Success`**。

*   **`Failure(string tag = null)`**: **强制失败节点**。执行其子节点，然后**强制返回 `Failure`**。

---

### 3. 动作与等待节点 (Action & Wait Nodes)

这些是行为树的叶子节点，执行具体操作或等待。

*   **`Do(string tag = null)`**: **通用动作节点**。最常用的节点，用于执行自定义逻辑。

    ```csharp
    behaviourTree
        .Do("Log Message", context => 
        {
            Console.WriteLine("Executing task!");
            return NodeResult.Success; // 执行完立即成功
        })
        .Do("Complex Logic", context => 
        {
            if (context.NeedsProcessing)
            {
                ProcessData();
                return NodeResult.Continue; // 需要下一帧继续
            }
            return NodeResult.Success; // 处理完成
        });
    ```

*   **`WaitTime(int ms, string tag = null)`**: **等待时间节点**。等待指定的毫秒数后返回 `Success`。

    ```csharp
    behaviourTree.WaitTime(2000); // 等待2秒
    ```

*   **`WaitTurn(int frameCount, string tag = null)`**: **等待帧数节点**。等待指定的帧数后返回 `Success`。

    ```csharp
    behaviourTree.WaitTurn(60); // 等待60帧 (通常约1秒)
    ```

*   **`WaitUntil(Func<T, bool> waitFunc, string tag = null)`**: **等待条件节点**。持续执行，直到 `waitFunc` 返回 `true`，然后返回 `Success`。

    ```csharp
    behaviourTree.WaitUntil(context => context.TargetReached);
    ```

---

## 重要方法

*   **`End()`**: **链式 API 的关键**。用于结束当前正在构建的复合节点或装饰节点，并返回到其父节点。**必须在每个 `Sequence`、`Select`、`Parallel`、`Invert` 等节点的末尾调用 `End()`**，否则树的结构会出错。
*   **`Reset()`**: 重置整个行为树的状态，使其回到初始状态，准备重新执行。
*   **`SetTag(string tag)`**: 为节点设置一个标签，主要用于调试日志。

---

## 调试 (Debugging)

框架内置了简单的调试日志功能。

```csharp
// 启用日志输出
BehaviourTreeDebug.Log = (message) => Debug.Log(message); // Unity 示例
// BehaviourTreeDebug.Log = (message) => Console.WriteLine(message); // 控制台示例

// 设置日志过滤器
BehaviourTreeDebug.Filter = BehaviourTreeDebug.LogFilter.Everything; // 记录所有事件
// BehaviourTreeDebug.Filter = BehaviourTreeDebug.LogFilter.OnStart | BehaviourTreeDebug.LogFilter.OnResult; // 只记录开始和结果
```

启用后，您将看到类似以下的日志：
```
Node:SequencerNode Tag:<null> OnStart Parameter:AIContext
Node:GenericNode Tag:Check Health OnUpdate Parameter:AIContext
Node:GenericNode Tag:Check Health Result:Success Parameter:AIContext
Node:WaitTimeNode Tag:<null> OnStart Parameter:AIContext
...
```

---

## 完整示例

```csharp
public class AIContext
{
    public float Health { get; set; } = 100;
    public float EnemyDistance { get; set; } = 10;
    public bool IsTaskComplete { get; set; } = false;
}

// 创建上下文和行为树
var context = new AIContext();
var tree = new BehaviourTree<AIContext>(context);

// 使用链式 API 构建复杂的 AI 行为
tree
    .Select("AI Main Logic") // 选择一个行为执行
        .Sequence("Combat Sequence") // 如果可以战斗
            .Do("Can Attack?", ctx => ctx.EnemyDistance < 3 ? NodeResult.Success : NodeResult.Failure)
            .Do("Attack!", ctx => { /* 攻击逻辑 */ return NodeResult.Success; })
        .End()
        .Sequence("Survival Sequence") // 否则尝试生存
            .Do("Low Health?", ctx => ctx.Health < 30 ? NodeResult.Success : NodeResult.Failure)
            .Do("Heal!", ctx => { ctx.Health += 20; return NodeResult.Success; })
            .WaitTime(1000) // 给治疗动画时间
        .End()
        .Do("Wander", ctx => { /* 随机移动 */ return NodeResult.Success; }) // 默认漫游
    .End();

// 在游戏循环中执行
while (true)
{
    tree.Execute();
    // 模拟时间流逝...
    System.Threading.Thread.Sleep(16); // ~60 FPS
}
```

通过这个流畅的链式 API，您可以非常直观和高效地定义复杂的行为逻辑。

---

## 📄 许可证

MIT License - 自由使用、修改与分发。

---

## 🙌 贡献

欢迎提交 Issue 与 Pull Request！  

---

> 💡 **提示**：链式 API 的核心是 `.End()` 返回上层，`.To()` 和 `.Return()` 完成转移配置。合理使用 `.SetTag()` 可提升日志可读性。
