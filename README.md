## What is GameplayTag?

GameplayTag is a C# implementation that mimics the GameplayTag plugin from Unreal Engine. Its functionality is nearly identical to that of Unreal Engine's GameplayTag system. Currently, it is designed to be used with Godot. However, if you wish to migrate it to Unity, the process should be relatively straightforward. You would mainly need to adapt the classes exposed to Godot to use Unity's API instead. Since I am not very familiar with Unity, there is no specific migration guide provided here for that engine. It is free and open-source forever!

GameplayTag是一个模仿虚幻引擎里面GameplayTag插件的C#版本。功能几乎与虚幻引擎里面的一致。目前是与Godot绑定使用，但如果你想迁移到Unity，改动也是很方便的，应该只需要把几个暴漏给Godot的类迁移使用Unity的API暴漏给Unity引擎使用，不过由于我对Unity不熟，所以这里没有相关的迁移指南。

## Concept Introduction
Currently, the project still lacks integration with the Godot editor, such as:

Maintaining the configuration definitions of tags through the Godot editor.
Directly configuring and maintaining GameplayTagContainer, GameplayTagCountContainer, GameplayTagRequirements, etc. on the Godot editor.
Therefore, anyone interested can refer to the project code and the Godot plugin writing guide to create a Godot plugin that adds and maintains tag configuration definitions.

目前该项目还缺少和Godot编辑器的集成，比如：
1. 通过Godot编辑器维护标签的配置定义
2. Godot编辑器上直接配置维护GameplayTagContainer，GameplayTagCountContainer，GameplayTagRequirements等

所以有兴趣的人，可以参考项目代码和Godot插件编写指南，自行弄一个Godot插件，添加配置和维护标签配置定义

### 使用上，了解下面三个类的使用方式和概念即可。
1. GameplayTagContainer
* GameplayTagContainer is a tag container used for storing and manipulating GameplayTags.
* GameplayTagContainer是一个标签容器，用于存储和操作GameplayTag。
2. GameplayTagCountContainer
* GameplayTagCountContainer is a tag stack count container used for storing and manipulating the stack counts of GameplayTags. It is utilized for managing game tags with event callbacks for tag count changes.
* GameplayTagCountContainer是一个标签堆叠数容器，用于存储和操作GameplayTag的堆叠数。用于管理带有标签计数更改的事件回调的游戏标签。
* One usage scenario: Managing the layer count of skill buffs. For example, A.B.C represents a poison DeBuff. The number of this tag indicates the number of DeBuff layers. Listening for corresponding count change events allows for handling the respective DeBuff logic.
* 使用场景之一：管理技能buff的层级次数，比如：A.B.C代表毒性DeBuff，那么这个标签有几个，就有几层Debuff，监听对应数量变化事件处理对应的Debuff逻辑，
3. GameplayTagRequirements
* GameplayTagRequirements is a tag requirement specifier used to describe a set of tag requirements. Its usage is illustrated in the advanced example below.
* GameplayTagRequirements是一个标签需求规则器，用于描述一个标签需求，使用方法如下面的高阶用法例子所示：
4. GameplayTagManager
* The tag manager is responsible for managing the loading and querying of tags. It's important to note that the tag configuration (such as the JSON configuration in the example) needs to be predefined. Currently, dynamically adding tags that do not exist in the configuration is not supported. (If I recall correctly, Unreal Engine also does not support this, as it can have an impact on performance.)
* 标签管理器，用于管理标签的加载和查询。需要注意，标签配置（例子里面的json配置），需要提前定义好，目前不支持动态添加配置中不存在的标签。（如果我没记错的话，虚幻引擎应该也不支持，因为这个对性能有一定影响。）

## Getting Started

1. 需要在启动时从配置文件中加载所有的标签，例如如下配置文件：
```json
[
    { "tag_name": "A.B.C", "description": "Description of A.B.C" },
    { "tag_name": "A.B.D", "description": "Description of A.B.D" },
    { "tag_name": "A.C", "description": "Description of A.C" },
    { "tag_name": "D", "description": "Description of D" },
    { "tag_name": "D.C", "description": "Description of D" },
    { "tag_name": "D.C.B", "description": "Description of D" },
    { "tag_name": "A.C.B", "description": "Description of D" }
]
```
```CSharp
GameplayTagsManager.Instance.ConstructGameplayTagTree(new GameplayTagsSettings());
```

2. GameplayTagContainer使用方式
```CSharp
GameplayTag tagABC = GameplayTag.RequestGameplayTag("A.B.C");
GameplayTagContainer containerA = new();
containerA.AddTag(tagABC);


GameplayTag tagAB = GameplayTag.RequestGameplayTag("A.B");
GameplayTag tagDCB = GameplayTag.RequestGameplayTag("D.C.B");
GameplayTagContainer containerB = GameplayTag.RequestGameplayTag();
containerB.AddTag(tagDCB);
containerB.AddTag(tagAB);


GameplayTagContainer containerC = new();
GameplayTag tagDCB2 = GameplayTag.RequestGameplayTag("D.C.B");
containerC.AddTag(tagDCB2);

var hasTag = containerA.HasTag(tagABC);  //true
var hasTagExact = containerA.HasTagExact(tagABC);  //true
var hasAny = containerA.HasAny(containerB);  //true
var hasAny2 = containerB.HasAnyExact(containerC); //true
var hasAll = containerC.HasAll(containerB); //false
containerB.RemoveTag(tagAB, false);
var hasAll2 = containerC.HasAll(containerB);  //true
var hasAllExact = containerB.HasAllExact(containerC);  //true
```

3. 高阶用法(GameplayTagRequirement与GameplayTagQuery的用法)
```CSharp
var require = new GameplayTagRequirement();
// 1. 自己或者父级需要有A.B.C
GameplayTag tagABC = GameplayTag.RequestGameplayTag("A.B.C");
require.RequireTags.AddTag(tagABC);
// 2. 自己或者父级不能有D.C.B
GameplayTag tagDCB = GameplayTag.RequestGameplayTag("D.C.B");
require.IgnoreTags.AddTag(tagDCB);
// 3. 自己或者父级有A.C
GameplayTag tagAC = GameplayTag.RequestGameplayTag("A.C");
require.TagQuery.Expr.AnyTagsMatch().AddTag(tagAC);


GameplayTag tagABC2 = GameplayTag.RequestGameplayTag("A.B.C");
GameplayTag tagAC2 = GameplayTag.RequestGameplayTag("A.C");
GameplayTagContainer containerA = new();
containerA.AddTag(tagABC2);
containerA.AddTag(tagAC2);
var result = require.RequirementsMet(containerA);
```

4. GameplayTagCountContainer的用法（带事件通知的标签容器）

```CSharp
//定义委托方法
private void onGameplayTagCountChanged(GameplayTag tag, int count)
{
    GD.Print($"onGameplayTagCountChanged: {tag}, {count}");
    InfoDisplay.Text += $"onGameplayTagCountChanged: {tag}, {count}\n";
}
//绑定委托，这个OnAnyTagChangeDelegate委托是当任意Tag新增或者溢出时触发的
GameplayTagCountContainer TagCountContainer = new();
TagCountContainer.OnAnyTagChangeDelegate = onGameplayTagCountChanged;

//添加标签堆栈数，
GameplayTag tagABC = GameplayTag.RequestGameplayTag("A.B.C");
var result = TagCountContainer.UpdateTagCount(tagABC, 1);
//负数就是减去
//var result = TagCountContainer.UpdateTagCount(tagABC, -1);


GameplayTag tagAB = GameplayTag.RequestGameplayTag("A.B");
GameplayTag tagDCB = GameplayTag.RequestGameplayTag("D.C.B");
GameplayTagContainer containerB = new();
containerB.AddTag(tagDCB);
containerB.AddTag(tagAB);
TagCountContainer.UpdateTagCount(containerB, 1);


//绑定监听某个容器里，特定标签的变化。两种变化：
// 1. 任何标签堆叠数量变化 AnyCountChange
// 2. 新增或删除某个标签的变化 NewOrRemove
GameplayTag tagABC = GameplayTag.RequestGameplayTag("A.B.C");
TagCountContainer.RegisterGameplayTagEvent(tagABC, EGameplayTagEventType.AnyCountChange, onSpecificTagCountChanged);

private void onSpecificTagCountChanged(GameplayTag tag, int count)
{
    GD.Print($"onSpecificTagCountChanged: {tag}, {count}");
}
```
