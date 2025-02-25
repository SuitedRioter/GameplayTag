

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Gameplay.Tag;


public class GameplayTagsManager
{
    
    
    public static GameplayTagsManager Instance
    {
        get { return _instance; }
    }
    
    public GameplayTagNode Root { get; set; }
    public Dictionary<GameplayTag, GameplayTagNode> TagMap { get; set; }
    
    private static readonly GameplayTagsManager _instance = new GameplayTagsManager();
    
    public GameplayTagsManager()
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
        for (int i = 0; i < parts.Length; i++)
        {
            var isExplicit = i == parts.Length - 1;
            var shortTagName = string.Intern(parts[i]);
            if (i == 0)
            {
                fullTagName = shortTagName;
            }
            else
            {
                fullTagName = string.Intern(fullTagName + "." + shortTagName);
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
        LoadFromJson(settings.JsonDataForTest);
    }

    private void LoadFromJson(string json)
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
        TagName = tagName;
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
        for (int i = 0; i < ChildTags.Count; i++)
        {
            if (string.Equals(ChildTags[i].TagName, tagName, StringComparison.Ordinal))
            {
                return i;
            }
        }
        return -1;
    }
}