## What is GameplayTag?

GameplayTag is a C # version that imitates the GameplayTag plugin in Unreal Engine. 
The functionality is almost identical to that in Unreal Engine. At present, it is bound and used with Godot, but if you want to migrate to Unity, 
it is also very convenient to make changes. You only need to refer to the code of the branch: csharp version, and migrate several classes that were leaked to Godot using Unity's API. 
However, since I am not familiar with Unity, there is no relevant migration guide here.

GameplayTag是一个模仿虚幻引擎里面GameplayTag插件的C#版本。功能几乎与虚幻引擎里面的一致。目前是与Godot绑定使用，但如果你想迁移到Unity，改动也是很方便的，只需要参考分支：csharp-version的代码，
把几个暴漏给Godot的类迁移使用Unity的API暴漏给Unity引擎使用，不过由于我对Unity不熟，所以这里没有相关的迁移指南。

## Concept Introduction

1. GameplayTagContainer

2. GameplayTagCountContainer

3. GameplayTagRequirements


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
配置文件地址？查看GameplayTagsSettings.cs，可自行定义
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