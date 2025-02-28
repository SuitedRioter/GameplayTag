using Godot;
using Gameplay.Tag;

public partial class Test : Node2D
{

	public Button AddCount;
	public Button RemoveCount;
	public Label InfoDisplay;
	
	public GameplayTagCountContainer TagCountContainer;
	
	public override void _Ready()
	{
		GameplayTagsManager.Instance.ConstructGameplayTagTree(new GameplayTagsSettings());
		AddCount = GetNode<Button>("addCount");
		RemoveCount = GetNode<Button>("removeCount");
		InfoDisplay = GetNode<Label>("InfoDisplay");
		
		TagCountContainer = new();
		TagCountContainer.OnAnyTagChangeDelegate = onGameplayTagCountChanged;

		AddCount.Pressed += addGameplayTagCount;
		RemoveCount.Pressed += removeGameplayTagCount;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//GameplayTagContainerTest();
		//GameplayTagQueryTest();
	}
	
	public void GameplayTagQueryTest()
	{
		var require = new GameplayTagRequirement();
		// 1. 自己或者父级需要有A.B.C
		GameplayTag tagABC = new("A.B.C");
		require.RequireTags.AddTag(tagABC);
		// 2. 自己或者父级不能有D.C.B
		GameplayTag tagDCB = new("D.C.B");
		require.IgnoreTags.AddTag(tagDCB);
		// 3. 自己或者父级有A.C
		GameplayTag tagAC = new("A.C");
		require.TagQuery.Expr.AnyTagsMatch().AddTag(tagAC);
		
		
		GameplayTag tagABC2 = new("A.B.C");
		GameplayTag tagAC2 = new("A.C");
		GameplayTagContainer containerA = new();
		containerA.AddTag(tagABC2);
		containerA.AddTag(tagAC2);
		var result = require.RequirementsMet(containerA);
		GD.Print($"requirementsMet: {result}");
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

	private void addGameplayTagCount()
	{
		GameplayTag tagABC = new("A.B.C");
		var result = TagCountContainer.UpdateTagCount(tagABC, 1);
		GD.Print($"addGameplayTagCount: {result}");
	}

	private void removeGameplayTagCount()
	{
		GameplayTag tagABC = new("A.B.C");
		var result = TagCountContainer.UpdateTagCount(tagABC, -1);
		GD.Print($"removeGameplayTagCount: {result}");
	}

	private void onGameplayTagCountChanged(GameplayTag tag, int count)
	{
		GD.Print($"onGameplayTagCountChanged: {tag}, {count}");
		InfoDisplay.Text = $"onGameplayTagCountChanged: {tag}, {count}";
	}
}
