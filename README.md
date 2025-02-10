## What is GameplayTag?

GameplayTag is a C# implementation that mimics the GameplayTag plugin from Unreal Engine. Its functionality is nearly identical to that of Unreal Engine's GameplayTag system. Currently, it is designed to be used with Godot. However, if you wish to migrate it to Unity, the process should be relatively straightforward. You would mainly need to adapt the classes exposed to Godot to use Unity's API instead. Since I am not very familiar with Unity, there is no specific migration guide provided here for that engine. It is free and open-source forever!


## Getting Started

```CSharp
use bevy::prelude::*;

fn main(){
  App::new()
    .add_plugins(DefaultPlugins)
    .run();
}
```
