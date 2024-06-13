using Godot;
using System;

public partial class npcSpawner : Node2D
{
	[Export]
	public PackedScene npcScene;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_calendar_day_changed()
	{
		if (npcScene != null)
		{
			Node newNpc = npcScene.Instantiate();
			newNpc.Set("z_as_relative", false);
			AddChild(newNpc);
		}
	}
}
