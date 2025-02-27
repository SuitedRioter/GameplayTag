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
    /// 并且这个容器是新对象，可以修改，不影响原始的容器。
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

    /// <summary>
    /// 把 GameplayTags（显式标签）里面Tag的父级标签，添加到自己的ParentTags（隐式标签）里面
    /// </summary>
    /// <param name="tag"></param>
    public void AddParentTag(GameplayTag tag)
    {
        var singleContainer = GameplayTagsManager.Instance.GetSingleTagContainer(tag);
        if (singleContainer != null)
        {
            for (int i = 0; i < singleContainer.ParentTags.Count; i++)
            {
                var parentTag = singleContainer.ParentTags[i];
                var index = ParentTags.BinarySearch(parentTag);
                if (index < 0)
                {
                    index = ~index;
                    ParentTags.Insert(index, parentTag);
                }
            }
        }
    }

    /// <summary>
    /// 根据自身的GameplayTags（显式标签），重新刷新填充ParentTags（隐式标签）
    /// 不出意外，这个有线程安全问题。可以自己确保单线程调用这个方法
    /// </summary>
    public void FillParentTags()
    {
        ParentTags.Clear();
        for (int i = 0; i < GameplayTags.Count; i++)
        {
            var tag = GameplayTags[i];
            AddParentTag(tag);
        }
    }

    public bool RemoveTag(GameplayTag tag, bool deferParentTags)
    {
        int index = FindTagIndex(tag);
        if (index >= 0)
        {
            GameplayTags.RemoveAt(index);
            if (!deferParentTags)
            {
                FillParentTags();
            }
        }
        return false;
    }

    public void AppendTags(GameplayTagContainer other)
    {
        for (int i = 0; i < other.GameplayTags.Count; i++)
        {
            var tag = other.GameplayTags[i];
            AddTag(tag);
        }
    }

    /// <summary>
    /// 返回自身GameplayTags（显式标签）和other GameplayTags（显式标签）的交集
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public GameplayTagContainer Filter(GameplayTagContainer other)
    {
        var result = new GameplayTagContainer();
        for (int i = 0; i < GameplayTags.Count; i++)
        {
            var tag = GameplayTags[i];
            if (tag.MatchesAny(other))
            {
                result.AddTagFast(tag);
            }
        }
        return result;
    }

    public GameplayTagContainer FilterExact(GameplayTagContainer other)
    {
        var result = new GameplayTagContainer();
        for (int i = 0; i < GameplayTags.Count; i++)
        {
            var tag = GameplayTags[i];
            if (tag.MatchesAnyExact(other))
            {
                result.AddTagFast(tag);
            }
        }
        return result;
    }


    public bool HasTag(GameplayTag tag)
    {
        return GameplayTags.BinarySearch(tag) >= 0 || ParentTags.BinarySearch(tag) >= 0;
    }

    public bool HasTagExact(GameplayTag tag)
    {
        return GameplayTags.BinarySearch(tag) >= 0;
    }

    public bool HasAny(GameplayTagContainer containerToCheck)
    {
        if (!containerToCheck.IsEmpty())
        {
            for (int i = 0; i < containerToCheck.GameplayTags.Count; i++)
            {
                var otherTag = containerToCheck.GameplayTags[i];
                if (HasTag(otherTag))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool HasAnyExact(GameplayTagContainer containerToCheck)
    {
        if (!containerToCheck.IsEmpty())
        {
            for (int i = 0; i < containerToCheck.GameplayTags.Count; i++)
            {
                var otherTag = containerToCheck.GameplayTags[i];
                if (HasTagExact(otherTag))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool HasAll(GameplayTagContainer containerToCheck)
    {
        if (!containerToCheck.IsEmpty())
        {
            for (int i = 0; i < containerToCheck.GameplayTags.Count; i++)
            {
                var otherTag = containerToCheck.GameplayTags[i];
                if (!HasTag(otherTag))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public bool HasAllExact(GameplayTagContainer containerToCheck)
    {
        if (!containerToCheck.IsEmpty())
        {
            for (int i = 0; i < containerToCheck.GameplayTags.Count; i++)
            {
                var otherTag = containerToCheck.GameplayTags[i];
                if (!HasTagExact(otherTag))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private int FindTagIndex(GameplayTag tag)
    {
        return GameplayTags.BinarySearch(tag);
    }
}

/// <summary>
/// 生成后，就不会修改，所以同名的标签，可以一直使用一个GameplayTag对象
/// </summary>
public struct GameplayTag : IEquatable<GameplayTag>, IComparable<GameplayTag>
{
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


public enum GameplayTagQueryExprType
{
    Undefined = 0,
    AnyTagsMatch,
    AllTagsMatch,
    NoTagsMatch,
    AnyExprMatch,
    AllExprMatch,
    NoExprMatch,
}


public class GameplayTagQueryExpression
{
    public GameplayTagQueryExprType ExprType { get; set; }
    public List<GameplayTagQueryExpression> ExprSet { get; set; }
    public List<GameplayTag> TagSet { get; set; }
    
}


public class GameplayTagQuery
{
    public GameplayTagQueryExpression Expr { get; set; }
    public string Description { get; set; }
}