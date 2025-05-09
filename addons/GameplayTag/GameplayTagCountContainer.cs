
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Godot;

namespace Gameplay.Tag;


public class GameplayTagCountContainer
{
    
    public Dictionary<GameplayTag, int> GameplayTagCountMap { get; set; } = new();
    public Dictionary<GameplayTag, int> ExplicitTagCountMap { get; set; } = new();
    public GameplayTagContainer ExplicitTags { get; set; } = new();
    public OnGameplayEffectTagCountChanged OnAnyTagChangeDelegate { get; set; }
    public Dictionary<GameplayTag, DelegateInfo> GameplayTagEventMap { get; set; } = new();


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

    
    /// <summary>
    /// 将某个Tag的堆栈数量加上countDelta（负数就是减去）
    /// 需要注意，还会给父Tag的堆栈数量加上countDelta（负数就是减去）
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="countDelta"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 将某个Tag的堆栈数量更新为newCount
    /// 注意Tag的父级也会
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="newCount"></param>
    /// <returns></returns>
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

    public void RegisterGameplayTagEvent(GameplayTag tag, EGameplayTagEventType eventType, OnGameplayEffectTagCountChanged callable)
    {
        ref var delegateInfo =
            ref CollectionsMarshal.GetValueRefOrAddDefault(GameplayTagEventMap, tag, out bool exists);
        if (!exists)
        {
            delegateInfo = new DelegateInfo();
        }
        
        if (eventType == EGameplayTagEventType.NewOrRemove)
        {
            delegateInfo.OnNewOrRemove += callable;
        }
        else
        {
            delegateInfo.OnAnyChange += callable;
        }
        
    }
    
    public OnGameplayEffectTagCountChanged RegisterGameplayTagEvent(GameplayTag tag, EGameplayTagEventType eventType)
    {
        ref var delegateInfo =
            ref CollectionsMarshal.GetValueRefOrAddDefault(GameplayTagEventMap, tag, out bool exists);
        if (!exists)
        {
            delegateInfo = new DelegateInfo();
        }
        
        if (eventType == EGameplayTagEventType.NewOrRemove)
        {
            return delegateInfo.OnNewOrRemove ;
        }
        return delegateInfo.OnAnyChange ;
    }
    
    public void UnRegisterGameplayTagEvent(GameplayTag tag, EGameplayTagEventType eventType, OnGameplayEffectTagCountChanged callable)
    {
        ref var delegateInfo =
            ref CollectionsMarshal.GetValueRefOrAddDefault(GameplayTagEventMap, tag, out bool exists);
        if (!exists)
        {
            delegateInfo = new DelegateInfo();
        }
        
        if (eventType == EGameplayTagEventType.NewOrRemove)
        {
            delegateInfo.OnNewOrRemove -= callable;
        }
        else
        {
            delegateInfo.OnAnyChange -= callable;
        }
        
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

        var existingCount = ExplicitTagCountMap.TryGetValue(tag, out var count)
            ? Math.Max(count + countDelta, 0)
            : Math.Max(countDelta, 0);
        
        ExplicitTagCountMap[tag] = existingCount;
        if (existingCount <= 0)
        {
            ExplicitTags.RemoveTag(tag, deferParentTagsOnRemove);
        }

        return true;
    }

    /// <summary>
    /// 收集tag自己和父级所有标签变化的通知集合
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
            var newCount = GameplayTagCountMap.TryGetValue(tagToCheck, out var count)
                ? Math.Max(count + countDelta, 0)
                : countDelta;
            //是否是重大更新，比如新增或者移除
            var significantChange = count == 0 || newCount == 0;
            GameplayTagCountMap[tagToCheck] = newCount;
            createdSignificantChange |= significantChange;
            if (significantChange)
            {
                tagChangeDelegates.Add(() =>
                {
                    OnAnyTagChangeDelegate?.Invoke(tagToCheck, newCount);
                });
            }

            if (GameplayTagEventMap.TryGetValue(tagToCheck, out var delegateInfo))
            {
                tagChangeDelegates.Add(() =>
                {
                    delegateInfo.OnAnyChange?.Invoke(tagToCheck, newCount);
                });
                    
                if (significantChange)
                {
                    tagChangeDelegates.Add(() =>
                    {
                        delegateInfo.OnNewOrRemove?.Invoke(tagToCheck, newCount);
                    });
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

public class DelegateInfo
{
    public OnGameplayEffectTagCountChanged OnNewOrRemove;
    public OnGameplayEffectTagCountChanged OnAnyChange;
}

public delegate void DeferredTagChangeDelegate();

public delegate void OnGameplayEffectTagCountChanged(GameplayTag tag, int count);