using Godot;

namespace Gameplay.Tag;

[GlobalClass]
public partial class GameplayTagQuery : Resource
{
    [Export]
    public GameplayTagQueryExpression Expr { get; set; } = new();
    [Export]
    public string Description { get; set; }

    public bool IsEmpty()
    {
        return Expr.IsValid();
    }

    public static GameplayTagQuery Build(GameplayTagQueryExpression expression)
    {
        return new GameplayTagQuery()
        {
            Expr = expression,
        };
    }

    //如果为空，我们认为就匹配任何标签
    public bool Matches(GameplayTagContainer container)
    {
        return IsEmpty() || Expr.Matches(container);
    }
}