using Godot;
using Godot.Collections;

namespace Gameplay.Tag;

[GlobalClass]
public partial class GameplayTagQueryExpression : Resource
{
    [Export]
    public GameplayTagQueryExprType ExprType { get; set; } = GameplayTagQueryExprType.Undefined;
    [Export]
    public Array<GameplayTagQueryExpression> ExprSet { get; set; } = new();
    [Export]
    public Array<GameplayTag> TagSet { get; set; } = new();

    public bool IsValid()
    {
        if (ExprType == GameplayTagQueryExprType.Undefined)
        {
            return false;
        }
        else
        {
            if (ExprSet.Count == 0 && TagSet.Count == 0)
            {
                return false;
            }

            return true;
        }
    }

    public GameplayTagQueryExpression AnyTagsMatch()
    {
        ExprType = GameplayTagQueryExprType.AnyTagsMatch;
        return this;
    }
    
    public GameplayTagQueryExpression AllTagsMatch()
    {
        ExprType = GameplayTagQueryExprType.AllTagsMatch;
        return this;
    }
    
    public GameplayTagQueryExpression NoTagsMatch()
    {
        ExprType = GameplayTagQueryExprType.NoTagsMatch;
        return this;
    }
    
    public GameplayTagQueryExpression AnyExprMatch()
    {
        ExprType = GameplayTagQueryExprType.AnyExprMatch;
        return this;
    }
    
    public GameplayTagQueryExpression AllExprMatch()
    {
        ExprType = GameplayTagQueryExprType.AllExprMatch;
        return this;
    }
    
    public GameplayTagQueryExpression NoExprMatch()
    {
        ExprType = GameplayTagQueryExprType.NoExprMatch;
        return this;
    }

    //是否使用tag设置来判断
    public bool UsesTagSet()
    {
        return ExprType == GameplayTagQueryExprType.AnyTagsMatch ||
               ExprType == GameplayTagQueryExprType.AllTagsMatch ||
               ExprType == GameplayTagQueryExprType.NoTagsMatch;
    }

    public GameplayTagQueryExpression AddTag(GameplayTag tag)
    {
        TagSet.Add(tag);
        return this;
    }

    public GameplayTagQueryExpression AddTags(GameplayTagContainer tags)
    {
        for (var i = 0; i < tags.GameplayTags.Count; i++)
        {
            TagSet.Add(tags.GameplayTags[i]);
        }
        return this;
    }

    ///是否使用表达式expr来判断
    public bool UsesExprSet()
    {
        return ExprType == GameplayTagQueryExprType.AnyExprMatch ||
               ExprType == GameplayTagQueryExprType.AllExprMatch ||
               ExprType == GameplayTagQueryExprType.NoExprMatch;
    }

    public GameplayTagQueryExpression AddExpr(GameplayTagQueryExpression expression)
    {
        ExprSet.Add(expression);
        return this;
    }

    public bool Matches(GameplayTagContainer container)
    {
        switch (ExprType)
        {
            case GameplayTagQueryExprType.AnyTagsMatch:
                for (var i = 0; i < TagSet.Count; i++)
                {
                    var tag = TagSet[i];
                    if (container.HasTag(tag))
                    {
                        return true;
                    }
                }
                return false;
            case GameplayTagQueryExprType.AllTagsMatch:
                for (var i = 0; i < TagSet.Count; i++)
                {
                    var tag = TagSet[i];
                    if (!container.HasTag(tag))
                    {
                        return false;
                    }
                }
                return true;
            case GameplayTagQueryExprType.NoTagsMatch:
                for (var i = 0; i < TagSet.Count; i++)
                {
                    var tag = TagSet[i];
                    if (container.HasTag(tag))
                    {
                        return false;
                    }
                }
                return true;
            case GameplayTagQueryExprType.AnyExprMatch:
                for (var i = 0; i < ExprSet.Count; i++)
                {
                    var expr = ExprSet[i];
                    if (expr.Matches(container))
                    {
                        return true;
                    }
                }
                return false;
            case GameplayTagQueryExprType.AllExprMatch:
                for (var i = 0; i < ExprSet.Count; i++)
                {
                    var expr = ExprSet[i];
                    if (!expr.Matches(container))
                    {
                        return false;
                    }
                }
                return true;
            case GameplayTagQueryExprType.NoExprMatch:
                for (var i = 0; i < ExprSet.Count; i++)
                {
                    var expr = ExprSet[i];
                    if (expr.Matches(container))
                    {
                        return false;
                    }
                }
                return true;
            default:
                return false;
                
        }
    }
}