## What is GameplayTag?

GameplayTag is a C# implementation that mimics the GameplayTag plugin from Unreal Engine. Its functionality is nearly identical to that of Unreal Engine's GameplayTag system. Currently, it is designed to be used with Godot. However, if you wish to migrate it to Unity, the process should be relatively straightforward. You would mainly need to adapt the classes exposed to Godot to use Unity's API instead. Since I am not very familiar with Unity, there is no specific migration guide provided here for that engine. It is free and open-source forever!

GameplayTag是一个模仿虚幻引擎里面GameplayTag插件的C#版本。功能几乎与虚幻引擎里面的一致。目前是与Godot绑定使用，但如果你想迁移到Unity，改动也是很方便的，应该只需要把几个暴漏给Godot的类迁移使用Unity的API暴漏给Unity引擎使用，不过由于我对Unity不熟，所以这里没有相关的迁移指南。

## Getting Started
本分支是初版，和Godot结合不深，使用的Api和方法大多是CSharp的原始方法，这样方便移植到其他引擎里，比如Unity里。

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
GameplayTag tagABC = new("A.B.C");
GameplayTagContainer containerA = new();
containerA.AddTag(tagABC);


GameplayTag tagAB = new("A.B");
GameplayTag tagDCB = new("D.C.B");
GameplayTagContainer containerB = new();
containerB.AddTag(tagDCB);
containerB.AddTag(tagAB);


GameplayTagContainer containerC = new();
GameplayTag tagDCB2 = new("D.C.B");
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

```

4. GameplayTagCountContainer的用法（带事件通知的标签容器）
```CSharp

```
