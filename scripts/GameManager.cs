using Godot;
using System;

public partial class GameManager : Node
{
	private Regions selectedRegion = null;
	
	public int CurrentPlayer = 0;
	public int ActionsLeft = 2;
	public int Stability = 20;
	

	public void UseAction()
	{
		ActionsLeft--;

		if (ActionsLeft <= 0)
			EndTurn();
	}

	public void EndTurn()
	{
		CurrentPlayer = (CurrentPlayer + 1) % 2; // 2 players for now
		ActionsLeft = 2;
		
		GD.Print("Next player: " + CurrentPlayer);
		UpdateUI();
	}
	
	public override void _Ready()
	{
		foreach (Regions region in GetTree().GetNodesInGroup("regions"))
		{
			region.RegionClicked += OnRegionSelected;
		}

		UpdateUI();
	}

	public void OnRegionSelected(Regions region)
	{
		if (selectedRegion == null)
		{
			selectedRegion = region;
			region.Highlight(true);
		}
		else
		{
			Expand(selectedRegion, region);

			selectedRegion.Highlight(false);
			selectedRegion = null;

			UseAction();
			UpdateUI();
		}
	}
	
	public void Expand(Regions from, Regions to)
	{
		if (!from.Neighbors.Contains(to))
		{
			GD.Print("Not adjacent!");
			return;
		}

		to.AddUnit(CurrentPlayer);
		ChangeStability(-1);
	}
	
	public void ChangeStability(int amount)
	{
		Stability += amount;
		GD.Print("Stability: " + Stability);

		if (Stability <= 0)
		{
			GD.Print("SYSTEM COLLAPSE - Everyone loses");
		}
	}
	

	public void UpdateUI()
	{
		GetNode<Label>("UI/TurnLabel").Text =
		$"P{CurrentPlayer} | Actions: {ActionsLeft} | Stability: {Stability}";
	}
	
}
