using Godot;
using System.Collections.Generic;

public partial class Regions : Node2D
{
	public int Owner = -1; // -1 = none
	public Dictionary<int, int> Units = new Dictionary<int, int>();
	public List<Regions> Neighbors = new List<Regions>();

	public bool Contested = false;
	
	[Signal]
	public delegate void RegionClickedEventHandler(Regions region);

	public void AddUnit(int player)
	{
		Units[player] += 1;
		UpdateState();
		
		UpdateVisual();                                                                      
		UpdateLabel();
	}

	public void UpdateState()
	{
		int playersWithUnits = 0;

		foreach (var kvp in Units)
		{
			if (kvp.Value > 0)
				playersWithUnits++;
		}

		Contested = playersWithUnits > 1;

		if (!Contested)
		{
			foreach (var kvp in Units)
			{
				if (kvp.Value > 0)
					Owner = kvp.Key;
			}
		}
		
		UpdateVisual();                                                                      
		UpdateLabel();
	}
	
	public void UpdateVisual()
	{
		var rect = GetNode<ColorRect>("ColorRect");

		if (Contested)
			rect.Color = new Color(1, 1, 0); // yellow
		else if (Owner == 0)
			rect.Color = new Color(0, 0.5f, 1); // blue
		else if (Owner == 1)
			rect.Color = new Color(1, 0.3f, 0.3f); // red
		else
			rect.Color = new Color(0.5f, 0.5f, 0.5f); // neutral
	}
	
	public void UpdateLabel()
	{
		var label = GetNode<Label>("Label");

		label.Text = $"P1: {Units[0]} | P2: {Units[1]}";
	}
	
		public override void _Ready()
		{
			GD.Print("Region ready");

			Units[0] = 0;
			Units[1] = 0;

			GetNode<Area2D>("Area2D").InputEvent += OnInputEvent;
			
			var regions = GetTree().GetNodesInGroup("regions");

			var r = new List<Regions>();
			foreach (Regions reg in regions)
				r.Add(reg);

			// example layout (grid-like)
			r[0].Neighbors.AddRange(new[] { r[1], r[5] });
			r[1].Neighbors.AddRange(new[] { r[0], r[2], r[4] });
			r[2].Neighbors.AddRange(new[] { r[1], r[3] });
			r[3].Neighbors.AddRange(new[] { r[2], r[4] });
			r[4].Neighbors.AddRange(new[] { r[1], r[3], r[5] });
			r[5].Neighbors.AddRange(new[] { r[0], r[4] });

			UpdateVisual();
			UpdateLabel();
		}
		
		public void Highlight(bool on)
		{
			var rect = GetNode<ColorRect>("ColorRect");

			if (on)
				rect.Modulate = new Color(0.5f, 1f, 0.5f); // green tint
			else
				rect.Modulate = new Color(1, 1, 1); // normal
		}

		private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
		{
			if (@event is InputEventMouseButton mouseEvent)
			{
				if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
				{
					GD.Print("Region Clicked");
					EmitSignal(SignalName.RegionClicked, this);
				}
			}
		}
}
