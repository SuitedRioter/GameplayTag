

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Gameplay.Tag;


public class GameplayTagsManager
{
    
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private static readonly Lazy<GameplayTagsManager> _lazyInstance = new(() => new GameplayTagsManager());

    // 公共静态属性，用于获取单例实例
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public static GameplayTagsManager Instance => _lazyInstance.Value;
    
    public GameplayTagNode Root { get; set; }
    public Dictionary<GameplayTag, GameplayTagNode> TagMap { get; set; }
    
    // 私有构造函数，防止外部实例化
    private GameplayTagsManager()
    {
        Root = new GameplayTagNode("root", "root", null, false);
        TagMap = new Dictionary<GameplayTag, GameplayTagNode>();
    }

    /// <summary>
    /// 添加一个tagNode
    /// </summary>
    /// <param name="tagName">标签全名</param>
    public void AddTagNode(string tagName)
    {
        var currentNode = Root;
        var parts = tagName.Split(".");
        var fullTagName = "";
        for (var i = 0; i < parts.Length; i++)
        {
            var isExplicit = i == parts.Length - 1;
            var shortTagName = parts[i];
            if (i == 0)
            {
                fullTagName = shortTagName;
            }
            else
            {
                fullTagName = fullTagName + "." + shortTagName;
            }

            InsertTagIntoNodeArray(shortTagName, fullTagName, ref currentNode, isExplicit);
        }
    }

    public void InsertTagIntoNodeArray(string shortTagName, string fullTagName, ref GameplayTagNode parentNode, bool isExplicit)
    {
        var childIndex = parentNode.FindChild(shortTagName);
        if (childIndex == -1)
        {
            var newNode = new GameplayTagNode(shortTagName, fullTagName, parentNode, isExplicit);
            parentNode.ChildTags.Add(newNode);
            TagMap.Add(new GameplayTag(fullTagName), newNode);
            parentNode = newNode;
        }
        else
        {
            parentNode = parentNode.ChildTags[childIndex];
            parentNode.IsExplicitTag = parentNode.IsExplicitTag || isExplicit;
        }
    }

    /// <summary>
    /// 查找 tag 的所有父级标签
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public GameplayTagContainer GetSingleTagContainer(GameplayTag tag)
    {
        if (TagMap.TryGetValue(tag, out var node))
        {
            return node.CompleteTagWithParents;
        }

        return null;
    }

    /// <summary>
    /// 暂时从测试json里，加载
    /// </summary>
    /// <param name="settings"></param>
    public void ConstructGameplayTagTree(GameplayTagsSettings settings)
    {
        loadFromJson(settings.JsonDataForTest);
    }

    private void loadFromJson(string json)
    {
        var list = JsonConvert.DeserializeObject<List<GameplayTagTableRow>>(json);
        if (list == null)
        {
            return;
        }
        
        foreach (var row in list)
        {
            AddTagNode(row.TagName);
        }
    }

    /// <summary>
    /// 返回容器的所有标签，包括显示标签和隐式标签。
    /// 需要注意，返回的容器只有 gameplay_tags 属性有值，就是标签全部放在 gameplay_tags 属性里面的。
    /// 并且这个容器是新对象，可以修改，不影响原始的容器。
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public GameplayTagContainer RequestGameplayTagParents(GameplayTag tag)
    {
        return GetSingleTagContainer(tag)?.GetGameplayTagParents() ?? new GameplayTagContainer();
    }
}


public struct GameplayTagTableRow
{
    //string.Intern() 字符串驻留 来获取同名字符串的引用。
    public string TagName { get; set; }
    public string Description { get; set;}
    
}


public class GameplayTagNode
{
    public string TagName { get; set; }
    public GameplayTagContainer CompleteTagWithParents { get; set; }
    public bool IsExplicitTag;
    public List<GameplayTagNode> ChildTags { get; set; }
    public GameplayTagNode ParentTag { get; set; }
    
    public GameplayTagNode(string tagName, string fullName, GameplayTagNode parentTag, bool isExplicitTag)
    {
        TagName = string.Intern(tagName);
        ParentTag = parentTag;
        IsExplicitTag = isExplicitTag;
        ChildTags = new List<GameplayTagNode>();
        CompleteTagWithParents = new GameplayTagContainer();

        CompleteTagWithParents.GameplayTags.Add(new GameplayTag(fullName));
        if (parentTag != null)
        {
            GameplayTagContainer parentContainer = parentTag.CompleteTagWithParents;
            if (!parentContainer.IsEmpty())
            {
                CompleteTagWithParents.ParentTags.Add(parentContainer.GameplayTags[0]);
                CompleteTagWithParents.ParentTags.AddRange(parentContainer.ParentTags);
            }
        }
    }

    // 查询childTags里面TagName相同的节点索引位置
    public int FindChild(string tagName)
    {
        for (var i = 0; i < ChildTags.Count; i++)
        {
            if (string.Equals(ChildTags[i].TagName, tagName, StringComparison.Ordinal))
            {
                return i;
            }
        }
        return -1;
    }
}