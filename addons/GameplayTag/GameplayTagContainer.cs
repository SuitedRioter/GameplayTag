using System;
using System.Collections.Generic;


namespace Gameplay.Tag;


public class GameplayTagContainer
{
    public List<GameplayTag> GameplayTags { get; set; } = [];
    public List<GameplayTag> ParentTags { get; set; } = [];


    public bool IsEmpty()
    {
        return GameplayTags == null || GameplayTags.Count == 0;
    }

    /// <summary>
    /// 返回容器的所有标签，包括显示标签和隐式标签。
    /// 需要注意，返回的容器只有 gameplay_tags 属性有值，就是标签全部放在 gameplay_tags 属性里面的。
    /// </summary>
    /// <returns></returns>
    public GameplayTagContainer GetGameplayTagParents()
    {
        var result = new GameplayTagContainer();
        result.GameplayTags = new List<GameplayTag>(GameplayTags);
        for (var i = 0; i < ParentTags.Count; i++)
        {
            var tag = ParentTags[i];
            var index = result.GameplayTags.BinarySearch(tag);
            if (index < 0)
            {
                index = ~index;
                result.GameplayTags.Insert(index, tag);
            }
        }
        return result;
    }

    public void AddTag(GameplayTag tag)
    {
        if (tag.IsValid())
        {
            var index = GameplayTags.BinarySearch(tag);
            if (index < 0)
            {
                index = ~index;
                GameplayTags.Insert(index, tag);
                AddParentTag(tag);
            }
        }
    }

    public void AddTagFast(GameplayTag tag)
    {
        var index = GameplayTags.BinarySearch(tag);
        if (index < 0)
        {
            index = ~index;
            GameplayTags.Insert(index, tag);
            AddParentTag(tag);
        }
    }

    public void AddParentTag(GameplayTag tag)
    {
        
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