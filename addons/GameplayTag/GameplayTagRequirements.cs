
namespace Gameplay.Tag;


public class GameplayTagRequirement
{
    // 这里面的Tag全部都要有
    public GameplayTagContainer RequireTags { get; set; } = new();
    // 这里面的Tag全部都不能有
    public GameplayTagContainer IgnoreTags { get; set; } = new();
    public GameplayTagQuery TagQuery { get; set; } = new();

    public bool IsEmpty()
    {
        return RequireTags.IsEmpty() && IgnoreTags.IsEmpty() && TagQuery.IsEmpty();
    }

    public bool RequirementsMet(GameplayTagContainer containerToCheck)
    {
        var hasRequireMet = containerToCheck.HasAll(RequireTags);
        var hasIgnoreMet = containerToCheck.HasAny(IgnoreTags);
        var hasQueryMet = TagQuery.IsEmpty() || TagQuery.Matches(containerToCheck);
        
        return hasRequireMet && !hasIgnoreMet && hasQueryMet;
    }

    public GameplayTagQuery ConvertTagFieldsToTagQuery()
    {
        var hasRequire = !RequireTags.IsEmpty();
        var hasIgnore = !IgnoreTags.IsEmpty();
        if (!hasRequire && !hasIgnore)
        {
            return new GameplayTagQuery();
        }

        var requireExpression = new GameplayTagQueryExpression();
        var ignoreExpression = new GameplayTagQueryExpression();
        var rootExpression = new GameplayTagQueryExpression();
        switch (hasRequire, hasIgnore)
        {
            case (true, true):
                requireExpression.AllTagsMatch().AddTags(RequireTags);
                ignoreExpression.NoTagsMatch().AddTags(IgnoreTags);
                rootExpression.AllExprMatch().AddExpr(requireExpression).AddExpr(ignoreExpression);
                break;
            case (true, _): // 使用通配符匹配任意hasIgnore值
                requireExpression.AllTagsMatch().AddTags(RequireTags);
                rootExpression.AllExprMatch().AddExpr(requireExpression);
                break;
            default:
                ignoreExpression.NoTagsMatch().AddTags(IgnoreTags);
                rootExpression.AllExprMatch().AddExpr(ignoreExpression);
                break;
        }

        var query = GameplayTagQuery.Build(rootExpression);
        return query;
    }

}