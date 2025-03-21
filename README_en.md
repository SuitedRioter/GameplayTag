# GameplayTag Project Introduction

## Project Overview

`GameplayTag` is a C# implementation that mimics the `GameplayTag` plugin from Unreal Engine. Its functionality is nearly identical to that of Unreal Engine's `GameplayTag` system. Currently, it is designed to be used with Godot. However, if you wish to migrate it to Unity, the process should be relatively straightforward. You would mainly need to adapt the classes exposed to Godot to use Unity's API instead. Since I am not very familiar with Unity, there is no specific migration guide provided here for that engine. It is free and open-source forever!

## Concept Introduction
Currently, the project still lacks integration with the Godot editor, such as:

1. Maintaining the configuration definitions of tags through the Godot editor.
2. Directly configuring and maintaining `GameplayTagContainer`, `GameplayTagCountContainer`, `GameplayTagRequirements`, etc. on the Godot editor.

Therefore, anyone interested can refer to the project code and the Godot plugin writing guide to create a Godot plugin that adds and maintains tag configuration definitions.

### Key Classes and Concepts
1. **GameplayTagContainer**
    - `GameplayTagContainer` is a tag container used for storing and manipulating `GameplayTags`.

2. **GameplayTagCountContainer**
    - `GameplayTagCountContainer` is a tag stack count container used for storing and manipulating the stack counts of `GameplayTags`. It is utilized for managing game tags with event callbacks for tag count changes.
    - **Usage Scenario**: Managing the layer count of skill buffs. For example, `A.B.C` represents a poison DeBuff. The number of this tag indicates the number of DeBuff layers. Listening for corresponding count change events allows for handling the respective DeBuff logic.

3. **GameplayTagRequirements**
    - `GameplayTagRequirements` is a tag requirement specifier used to describe a set of tag requirements. Its usage is illustrated in the advanced example below.

4. **GameplayTagManager**
    - The tag manager is responsible for managing the loading and querying of tags. It's important to note that the tag configuration (such as the JSON configuration in the example) needs to be predefined. Currently, dynamically adding tags that do not exist in the configuration is not supported. (If I recall correctly, Unreal Engine also does not support this, as it can have an impact on performance.)

## Getting Started

### 1. Loading Tags from Configuration
You need to load all tags from a configuration file at startup. For example:

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
```csharp
GameplayTagsManager.Instance.ConstructGameplayTagTree(new GameplayTagsSettings());
```


### 2. Using `GameplayTagContainer`
```csharp
GameplayTag tagABC = GameplayTag.RequestGameplayTag("A.B.C");
GameplayTagContainer containerA = new();
containerA.AddTag(tagABC);

GameplayTag tagAB = GameplayTag.RequestGameplayTag("A.B");
GameplayTag tagDCB = GameplayTag.RequestGameplayTag("D.C.B");
GameplayTagContainer containerB = new();
containerB.AddTag(tagDCB);
containerB.AddTag(tagAB);

GameplayTagContainer containerC = new();
GameplayTag tagDCB2 = GameplayTag.RequestGameplayTag("D.C.B");
containerC.AddTag(tagDCB2);

var hasTag = containerA.HasTag(tagABC);  // true
var hasTagExact = containerA.HasTagExact(tagABC);  // true
var hasAny = containerA.HasAny(containerB);  // true
var hasAny2 = containerB.HasAnyExact(containerC); // true
var hasAll = containerC.HasAll(containerB); // false
containerB.RemoveTag(tagAB, false);
var hasAll2 = containerC.HasAll(containerB);  // true
var hasAllExact = containerB.HasAllExact(containerC);  // true
```


### 3. Advanced Usage (`GameplayTagRequirement` and `GameplayTagQuery`)
```csharp
var require = new GameplayTagRequirement();
// 1. Self or parent needs to have A.B.C
GameplayTag tagABC = GameplayTag.RequestGameplayTag("A.B.C");
require.RequireTags.AddTag(tagABC);
// 2. Self or parent must not have D.C.B
GameplayTag tagDCB = GameplayTag.RequestGameplayTag("D.C.B");
require.IgnoreTags.AddTag(tagDCB);
// 3. Self or parent has A.C
GameplayTag tagAC = GameplayTag.RequestGameplayTag("A.C");
require.TagQuery.Expr.AnyTagsMatch().AddTag(tagAC);

GameplayTag tagABC2 = GameplayTag.RequestGameplayTag("A.B.C");
GameplayTag tagAC2 = GameplayTag.RequestGameplayTag("A.C");
GameplayTagContainer containerA = new();
containerA.AddTag(tagABC2);
containerA.AddTag(tagAC2);
var result = require.RequirementsMet(containerA);
```


### 4. Using `GameplayTagCountContainer` (Tag Container with Event Notifications)
```csharp
// Define delegate method
private void onGameplayTagCountChanged(GameplayTag tag, int count)
{
    GD.Print($"onGameplayTagCountChanged: {tag}, {count}");
    InfoDisplay.Text += $"onGameplayTagCountChanged: {tag}, {count}\n";
}

// Bind delegate, the OnAnyTagChangeDelegate is triggered when any tag is added or removed
GameplayTagCountContainer tagCountContainer = new();
tagCountContainer.OnAnyTagChangeDelegate = onGameplayTagCountChanged;

// Add tag stack count
GameplayTag tagABC = GameplayTag.RequestGameplayTag("A.B.C");
var result = tagCountContainer.UpdateTagCount(tagABC, 1);

// Bind to listen for specific tag changes in a container. Two types of changes:
// 1. Any tag stack count change (AnyCountChange)
// 2. New tag added or existing tag removed (NewOrRemove)
GameplayTag tagABC = GameplayTag.RequestGameplayTag("A.B.C");
tagCountContainer.RegisterGameplayTagEvent(tagABC, EGameplayTagEventType.AnyCountChange, onSpecificTagCountChanged);

private void onSpecificTagCountChanged(GameplayTag tag, int count)
{
    GD.Print($"onSpecificTagCountChanged: {tag}, {count}");
}
```


## Conclusion

The `GameplayTag` library provides a powerful and flexible tag management system for game development, helping developers better manage and query various tags in games. Through this library, developers can easily implement complex tag logic, improving the maintainability and scalability of games.