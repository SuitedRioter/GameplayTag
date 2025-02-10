## What is GameplayTag?

GameplayTag is a C# implementation that mimics the GameplayTag plugin from Unreal Engine. Its functionality is nearly identical to that of Unreal Engine's GameplayTag system. Currently, it is designed to be used with Godot. However, if you wish to migrate it to Unity, the process should be relatively straightforward. You would mainly need to adapt the classes exposed to Godot to use Unity's API instead. Since I am not very familiar with Unity, there is no specific migration guide provided here for that engine. It is free and open-source forever!

GameplayTag是一个模仿虚幻引擎里面GameplayTag插件的C#版本。功能几乎与虚幻引擎里面的一致。目前是与Godot绑定使用，但如果你想迁移到Unity，改动也是很方便的，应该只需要把几个暴漏给Godot的类迁移使用Unity的API暴漏给Unity引擎使用，不过由于我对Unity不熟，所以这里没有相关的迁移指南。

## Getting Started

```CSharp
use bevy::prelude::*;

fn main(){
  App::new()
    .add_plugins(DefaultPlugins)
    .run();
}
```
