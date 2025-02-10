using System;
using Godot;


namespace Gameplay.Tag;


public class GameplayTagContainer
{
    
}


public struct GameplayTag
{
    private string tagName;

    public GameplayTag(string _tagName)
    {
        tagName = _tagName;
    }

    public string GetTagName()
    {
        return tagName;
    }

    public bool IsValid()
    {
        return tagName != null && tagName != "";
    }
}