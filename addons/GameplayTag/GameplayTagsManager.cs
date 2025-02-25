

using System;
using System.Collections.Generic;

namespace Gameplay.Tag;


public class GameplayTagsManager
{
    public GameplayTagNode Root { get; set; }
    public Dictionary<GameplayTag, GameplayTagNode> TagMap { get; set; }
    
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
        
    }

    public void InsertTagIntoNodeArray(string shortTagName, string fullTagName, GameplayTagNode parentNode, bool isExplicit)
    {
        
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