# GameplayTag 项目介绍

## 项目概述

`GameplayTag` 是一个模仿虚幻引擎中 `GameplayTag` 插件的 C# 实现，功能几乎与虚幻引擎中的一致。该项目目前与 Godot 引擎绑定使用，但也可以轻松迁移到 Unity 等其他引擎。通过该库，开发者可以灵活地管理和查询游戏中的标签，适用于游戏对象、技能、状态等的精细控制。

### 主要功能
1. **标签管理**：支持创建、添加、删除和查询标签。
2. **标签容器**：提供 `GameplayTagContainer` 类，用于存储和操作一组标签。
3. **标签计数**：`GameplayTagCountContainer` 类用于管理标签的计数和事件通知。
4. **标签需求规则**：`GameplayTagRequirements` 类用于描述复杂的标签需求规则。
5. **标签树结构**：通过 `GameplayTagsManager` 和 `GameplayTagNode` 类，支持标签的树形结构管理。

## 使用指南

### 1. 初始化标签管理器
首先，需要初始化 `GameplayTagsManager`，并加载标签数据。

```csharp
var settings = new GameplayTagsSettings();
GameplayTagsManager.Instance.ConstructGameplayTagTree(settings);
```


### 2. 创建和请求标签
使用 `GameplayTag` 结构体创建或请求标签。

```csharp
var tag = GameplayTag.RequestGameplayTag("A.B.C");
```


### 3. 使用标签容器
`GameplayTagContainer` 用于存储和操作一组标签。

```csharp
var container = new GameplayTagContainer();
container.AddTag(tag);
```


### 4. 标签查询
通过 `GameplayTagQuery` 进行复杂的标签查询。

```csharp
var query = new GameplayTagQuery();
query.Expr.AllTagsMatch().AddTag(tag);
bool result = query.Matches(container);
```


### 5. 标签计数
`GameplayTagCountContainer` 用于管理标签的计数和事件通知。

```csharp
var countContainer = new GameplayTagCountContainer();
countContainer.UpdateTagCount(tag, 1);
```


### 6. 标签树结构
`GameplayTagsManager` 和 `GameplayTagNode` 类支持标签的树形结构管理。

```csharp
var node = GameplayTagsManager.Instance.GetSingleTagContainer(tag);
```


## 示例代码

以下是一个简单的示例，展示如何使用 `GameplayTag` 库：

```csharp
// 初始化标签管理器
var settings = new GameplayTagsSettings();
GameplayTagsManager.Instance.ConstructGameplayTagTree(settings);

// 创建标签
var tag = GameplayTag.RequestGameplayTag("A.B.C");

// 使用标签容器
var container = new GameplayTagContainer();
container.AddTag(tag);

// 标签查询
var query = new GameplayTagQuery();
query.Expr.AllTagsMatch().AddTag(tag);
bool result = query.Matches(container);

// 标签计数
var countContainer = new GameplayTagCountContainer();
countContainer.UpdateTagCount(tag, 1);
```


## 高阶用法

### 1. 标签需求规则 (`GameplayTagRequirements`)
```csharp
var require = new GameplayTagRequirement();
// 1. 自己或者父级需要有 A.B.C
GameplayTag tagABC = GameplayTag.RequestGameplayTag("A.B.C");
require.RequireTags.AddTag(tagABC);
// 2. 自己或者父级不能有 D.C.B
GameplayTag tagDCB = GameplayTag.RequestGameplayTag("D.C.B");
require.IgnoreTags.AddTag(tagDCB);
// 3. 自己或者父级有 A.C
GameplayTag tagAC = GameplayTag.RequestGameplayTag("A.C");
require.TagQuery.Expr.AnyTagsMatch().AddTag(tagAC);

GameplayTag tagABC2 = GameplayTag.RequestGameplayTag("A.B.C");
GameplayTag tagAC2 = GameplayTag.RequestGameplayTag("A.C");
GameplayTagContainer containerA = new();
containerA.AddTag(tagABC2);
containerA.AddTag(tagAC2);
var result = require.RequirementsMet(containerA);
```


### 2. 带事件通知的标签容器 (`GameplayTagCountContainer`)
```csharp
// 定义委托方法
private void onGameplayTagCountChanged(GameplayTag tag, int count)
{
    GD.Print($"onGameplayTagCountChanged: {tag}, {count}");
    InfoDisplay.Text += $"onGameplayTagCountChanged: {tag}, {count}\n";
}

// 绑定委托，这个 OnAnyTagChangeDelegate 委托是当任意 Tag 新增或者溢出时触发的
GameplayTagCountContainer tagCountContainer = new();
tagCountContainer.OnAnyTagChangeDelegate = onGameplayTagCountChanged;

// 添加标签堆栈数
GameplayTag tagABC = GameplayTag.RequestGameplayTag("A.B.C");
var result = tagCountContainer.UpdateTagCount(tagABC, 1);

// 绑定监听某个容器里，特定标签的变化。两种变化：
// 1. 任何标签堆叠数量变化 AnyCountChange
// 2. 新增或删除某个标签的变化 NewOrRemove
GameplayTag tagABC = GameplayTag.RequestGameplayTag("A.B.C");
tagCountContainer.RegisterGameplayTagEvent(tagABC, EGameplayTagEventType.AnyCountChange, onSpecificTagCountChanged);

private void onSpecificTagCountChanged(GameplayTag tag, int count)
{
    GD.Print($"onSpecificTagCountChanged: {tag}, {count}");
}
```


## 结论

`GameplayTag` 库为游戏开发提供了一个强大且灵活的标签管理系统，帮助开发者更好地管理和查询游戏中的各种标签。通过该库，开发者可以轻松实现复杂的标签逻辑，提升游戏的可维护性和扩展性。