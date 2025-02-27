using NUnit.Framework;

namespace Gameplay.Tag.Test;

[TestFixture]
public class GameplayTagTest
{
    [Test]
    public void GameplayTagContainerTest()
    {
        initGameplayTagManager();
        
        GameplayTag tagABC = new("A.B.C");
        GameplayTagContainer containerA = new();
        containerA.AddTag(tagABC);
        
        
        GameplayTag tagAB = new("A.B");
        GameplayTag tagDCB = new("D.C.B");
        GameplayTagContainer containerB = new();
        containerB.AddTag(tagDCB);
        containerB.AddTag(tagAB);
        
        
        GameplayTagContainer containerC = new();
        GameplayTag tagDCB2 = new("D.C.B");
        containerC.AddTag(tagDCB2);

        var hasTag = containerA.HasTag(tagABC);
        var hasTagExact = containerA.HasTagExact(tagABC);
        var hasAny = containerA.HasAny(containerB);
        var hasAny2 = containerB.HasAnyExact(containerC);
        var hasAll = containerC.HasAll(containerB);
        containerB.RemoveTag(tagABC, false);
        var hasAll2 = containerC.HasAll(containerB);
        var hasAllExact = containerB.HasAllExact(containerC);

        Assert.Equals(true, hasTag);
        Assert.Equals(false, hasTagExact);
        Assert.Equals(true, hasAny);
        Assert.Equals(true, hasAny2);
        Assert.Equals(true, hasAll);
        Assert.Equals(false, hasAll2);
        Assert.Equals(true, hasAllExact);
    }

    private void initGameplayTagManager()
    {
        GameplayTagsManager.Instance.ConstructGameplayTagTree(new GameplayTagsSettings());
    }
}