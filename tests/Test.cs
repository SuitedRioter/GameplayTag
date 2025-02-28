using Gameplay.Tag;
using Godot;

public partial class Test : Node2D
{
	[Export] public GameplayTagRequirement Requirement;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GameplayTagsManager.Instance.ConstructGameplayTagTree(new GameplayTagsSettings());
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GameplayTagContainerTest();
	}
	
	
	public void GameplayTagContainerTest()
	{
        
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

		var hasTag = containerA.HasTag(tagABC);  //true
		var hasTagExact = containerA.HasTagExact(tagABC);  //true
		var hasAny = containerA.HasAny(containerB);  //true
		var hasAny2 = containerB.HasAnyExact(containerC); //true
		var hasAll = containerC.HasAll(containerB); //false
		containerB.RemoveTag(tagAB, false);
		var hasAll2 = containerC.HasAll(containerB);  //true
		var hasAllExact = containerB.HasAllExact(containerC);  //true
		
		
		GD.Print($"hasTag: {hasTag}, hasTagExact: {hasTagExact}, hasAny: {hasAny}, hasAny2: {hasAny2}, hasAll: {hasAll}, hasAll2: {hasAll2}, hasAllExact: {hasAllExact}");
        
	}
}
