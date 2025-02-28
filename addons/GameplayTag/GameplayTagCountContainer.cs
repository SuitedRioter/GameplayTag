
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Gameplay.Tag;



public class GameplayTagCountContainer 
{
    public Dictionary<GameplayTag, int> GameplayTagCountMap { get; set; } = new();
    public Dictionary<GameplayTag, int> ExplicitTagCountMap { get; set; } = new();
    public GameplayTagContainer ExplicitTags { get; set; } = new();
    public OnGameplayEffectTagCountChanged OnAnyTagChangeDelegate { get; set; }
    public Dictionary<GameplayTag, DelegateInfo> GameplayTagEventMap { get; set; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasMatchingGameplayTag(GameplayTag tagToCheck)
    {
        if (GameplayTagCountMap.TryGetValue(tagToCheck, out var count))
        {
            return count > 0;
        }
        return false;
    }


    public bool HasAllMatchingGameplayTags(GameplayTagContainer tagContainer)
    {
        if (tagContainer.IsEmpty())
        {
            return false;
        }

        var allMatch = true;
        for (var i = 0; i < tagContainer.GameplayTags.Count; i++)
        {
            var tag = tagContainer.GameplayTags[i];
            if (!HasMatchingGameplayTag(tag))
            {
                allMatch = false;
                break;
            }
        }

        return allMatch;
    }


    public bool HasAnyMatchingGameplayTags(GameplayTagContainer tagContainer)
    {
        if (tagContainer.IsEmpty())
        {
            return false;
        }

        var anyMatch = false;
        for (var i = 0; i < tagContainer.GameplayTags.Count; i++)
        {
            var tag = tagContainer.GameplayTags[i];
            if (HasMatchingGameplayTag(tag))
            {
                anyMatch = true;
                break;
            }
        }

        return anyMatch;
    }


    public void UpdateTagCount(GameplayTagContainer container, int countDelta)
    {
        if (countDelta != 0)
        {
            var updatedAny = false;
            var tagChangeDelegates = new List<DeferredTagChangeDelegate>();
            for (var i = 0; i < container.GameplayTags.Count; i++)
            {
                var tag = container.GameplayTags[i];
                updatedAny |= updateTagMapDeferredParentRemovalInternal(tag, countDelta, tagChangeDelegates);
            }

            if (updatedAny && countDelta < 0)
            {
                ExplicitTags.FillParentTags();
            }

            for (var i = 0; i < tagChangeDelegates.Count; i++)
            {
                var tagChangeDelegate = tagChangeDelegates[i];
                tagChangeDelegate();
            }
        }
    }



    public bool UpdateTagCount(GameplayTag tag, int countDelta)
    {
        if (countDelta != 0)
        {
            return updateTagMapInternal(tag, countDelta);
        }

        return false;
    }


    public bool UpdateTagCountDeferredParentRemoval(GameplayTag tag, int countDelta, List<DeferredTagChangeDelegate> tagChangeDelegates)
    {
        if (countDelta != 0)
        {
            return updateTagMapDeferredParentRemovalInternal(tag, countDelta, tagChangeDelegates);
        }

        return false;
    }


    public bool SetTagCount(GameplayTag tag, int newCount)
    {
        var existingCount = 0;
        if (ExplicitTagCountMap.TryGetValue(tag, out var count))
        {
            existingCount = count;
        }

        var countDelta = newCount - existingCount;
        if (countDelta != 0)
        {
            return updateTagMapInternal(tag, countDelta);  
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetTagCount(GameplayTag tag)
    {
        return ExplicitTagCountMap.GetValueOrDefault(tag, 0);
    }

    public void NotifyStackCountChange(GameplayTag tag)
    {
        var tagAndParentsContainer = GameplayTagsManager.Instance.GetSingleTagContainer(tag);
        for (var i = 0; i < tagAndParentsContainer.GameplayTags.Count; i++)
        {
            var tagToCheck = tagAndParentsContainer.GameplayTags[i];
            if (GameplayTagEventMap.TryGetValue(tagToCheck, out var delegateInfo))
            {
                int tagCount = 0;
                if (GameplayTagCountMap.TryAdd(tagToCheck, tagCount))
                {
                    tagCount = GameplayTagCountMap[tagToCheck];
                }
                delegateInfo.OnAnyChange?.Invoke(tagToCheck, tagCount);
            }
        }
    }

    public OnGameplayEffectTagCountChanged RegisterGameplayTagEvent(GameplayTag tag, EGameplayTagEventType eventType)
    {
        var delegateInfo = new DelegateInfo();
        if (GameplayTagEventMap.TryAdd(tag, delegateInfo))
        {
            delegateInfo = GameplayTagEventMap[tag];
        }

        return eventType == EGameplayTagEventType.NewOrRemove ? delegateInfo.OnNewOrRemove : delegateInfo.OnAnyChange;
        
    }

    public void Reset(bool resetEvents)
    {
        GameplayTagCountMap.Clear();
        ExplicitTagCountMap.Clear();
        ExplicitTags.Reset();
        if (resetEvents)
        {
            GameplayTagEventMap.Clear();
            OnAnyTagChangeDelegate = null;
        }
    }

    public void FillParentTags()
    {
        ExplicitTags.FillParentTags();
    }

    private bool updateTagMapInternal(GameplayTag tag, int countDelta)
    {
        if (!updateExplicitTags(tag, countDelta, false))
        {
            return false;
        }
        
        var tagChangeDelegates = new List<DeferredTagChangeDelegate>();
        var significantChange = gatherTagChangeDelegates(tag, countDelta, tagChangeDelegates);
        for (var i = 0; i < tagChangeDelegates.Count; i++)
        {
            var tagChangeDelegate = tagChangeDelegates[i];
            tagChangeDelegate();
        }

        return significantChange;
    }


    private bool updateTagMapDeferredParentRemovalInternal(GameplayTag tag, int countDelta, List<DeferredTagChangeDelegate> tagChangeDelegates)
    {
        if (!updateExplicitTags(tag, countDelta, true))
        {
            return false;
        }
        return gatherTagChangeDelegates(tag, countDelta, tagChangeDelegates);
    }

    private bool updateExplicitTags(GameplayTag tag, int countDelta, bool deferParentTagsOnRemove)
    {
        var tagAlreadyExplicitlyExists = ExplicitTags.HasTagExact(tag);
        if (!tagAlreadyExplicitlyExists)
        {
            if (countDelta > 0)
            {
                ExplicitTags.AddTag(tag);
            }
            else
            {
                //表示没有这个tag，但是如果有这个tag的子级，就发个警告日志。
                if (ExplicitTags.HasTag(tag))
                {
                   //($"{tag} is already in the explicit tag list, but it has children. This is not allowed.");
                }
                return false;
            }
        }

        if (ExplicitTagCountMap.TryGetValue(tag, out var count))
        {
            count = Math.Max(count + countDelta, 0);
            ExplicitTagCountMap[tag] = count;
            if (count <= 0)
            {
                ExplicitTags.RemoveTag(tag, deferParentTagsOnRemove);
            }
        }

        return true;
    }

    /// <summary>
    /// 这里有3个事件通知
    /// 1. 这个container自身有一个事件通知，就是这个这个容器任何变化，都会发送这个事件
    /// 2. 这个容器内，某个标签的2种通知，就是EventType的那两种，在特定时刻去发送。
    /// 这里只是定义了委托，并没有发送事件
    /// </summary>
    /// <returns></returns>
    private bool gatherTagChangeDelegates(GameplayTag tag, int countDelta, List<DeferredTagChangeDelegate> tagChangeDelegates)
    {
        var tagAndParentsContainer = GameplayTagsManager.Instance.RequestGameplayTagParents(tag);
        var createdSignificantChange = false;
        for (var i = 0; i < tagAndParentsContainer.GameplayTags.Count; i++)
        {
            var tagToCheck = tagAndParentsContainer.GameplayTags[i];
            if (GameplayTagCountMap.TryGetValue(tagToCheck, out var count))
            {
                var newCount = Math.Max(count + countDelta, 0);
                var significantChange = count == 0 || newCount == 0;
                GameplayTagCountMap[tagToCheck] = newCount;
                createdSignificantChange |= significantChange;
                if (significantChange)
                {
                    tagChangeDelegates.Add(() =>
                    {
                        OnAnyTagChangeDelegate?.Invoke(tag, newCount);
                    });
                }

                if (GameplayTagEventMap.TryGetValue(tagToCheck, out var delegateInfo))
                {
                    
                    tagChangeDelegates.Add(() =>
                    {
                        delegateInfo.OnAnyChange?.Invoke(tag, newCount);
                    });
                    
                    if (significantChange)
                    {
                        tagChangeDelegates.Add(() =>
                        {
                            delegateInfo.OnNewOrRemove?.Invoke(tag, newCount);
                        });
                    }
                }
            }
        }

        return createdSignificantChange;
    }
}

public enum EGameplayTagEventType
{
    NewOrRemove = 0,
    AnyCountChange = 1,
}

public struct DelegateInfo
{
    public OnGameplayEffectTagCountChanged OnNewOrRemove { get;set; }
    public OnGameplayEffectTagCountChanged OnAnyChange { get; set; }
};

public delegate void DeferredTagChangeDelegate();

public delegate void OnGameplayEffectTagCountChanged(GameplayTag tag, int count);