using Godot;
using System;

public partial class SceneManager : Node
{
	[Export]
	public PackedScene gamescene;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var scene = gamescene.Instantiate();
		AddChild(scene);
		
	}

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
		if (@event.IsAction("Exit"))
		{
			foreach (var child in GetChildren())
			{
				RemoveChild(child);
			}
		}
    }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
