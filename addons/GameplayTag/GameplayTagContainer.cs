using System;
using System.Collections.Generic;


namespace Gameplay.Tag;


public class GameplayTagContainer
{
    public List<GameplayTag> GameplayTags { get; set; }
    public List<GameplayTag> ParentTags { get; set; }


    public bool IsEmpty()
    {
        return GameplayTags == null || GameplayTags.Count == 0;
    }
}


public struct GameplayTag : IEquatable<GameplayTag>, IComparable<GameplayTag>
{
    public string TagName { get; set; }

    public GameplayTag(string tagName)
    {
        TagName = tagName;
    }

    public bool IsValid()
    {
        return !string.IsNullOrEmpty(TagName);
    }

    public bool Equals(GameplayTag other)
    {
        return TagName == other.TagName;
    }

    public override bool Equals(object obj)
    {
        if (obj is GameplayTag other)
        {
            return TagName == other.TagName;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return TagName?.GetHashCode() ?? 0;
    }
    
    public static bool operator ==(GameplayTag left, GameplayTag right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(GameplayTag left, GameplayTag right)
    {
        return !(left == right);
    }
    
    public int CompareTo(GameplayTag other)
    {
        return string.Compare(TagName, other.TagName, StringComparison.OrdinalIgnoreCase);
    }
    
    public override string ToString()
    {
        return TagName;
    }
}