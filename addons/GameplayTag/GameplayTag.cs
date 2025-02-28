using System;
using Godot;

namespace Gameplay.Tag;

/// <summary>
/// 生成后，就不会修改，所以同名的标签，可以一直使用一个GameplayTag对象
/// </summary>
[GlobalClass]
public partial class GameplayTag : Resource, IEquatable<GameplayTag>, IComparable<GameplayTag>
{
    [Export]
    public string TagName { get; private set; }

    public GameplayTag(string tagName)
    {
        TagName = string.Intern(tagName);
    }

    public bool IsValid()
    {
        return !string.IsNullOrEmpty(TagName);
    }


    public bool MatchesTag(GameplayTag tagToCheck)
    {
        var completeContainer = GameplayTagsManager.Instance.GetSingleTagContainer(this);
        if (completeContainer != null)
        {
            return completeContainer.HasTag(tagToCheck);
        }
        return false;
    }

    public bool MatchesTagExact(GameplayTag tagToCheck)
    {
        if (tagToCheck.IsValid())
        {
            return TagName == tagToCheck.TagName;
        }

        return false;
    }

    public bool MatchesAny(GameplayTagContainer containerToCheck)
    {
        var completeContainer = GameplayTagsManager.Instance.GetSingleTagContainer(this);
        if (completeContainer != null)
        {
            return completeContainer.HasAny(containerToCheck);
        }
        return false;
    }

    public bool MatchesAnyExact(GameplayTagContainer containerToCheck)
    {
        if (!containerToCheck.IsEmpty())
        {
            return containerToCheck.GameplayTags.BinarySearch(this) >= 0;
        }
        return false;
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