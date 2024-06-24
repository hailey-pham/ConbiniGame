using Godot;
using Godot.Collections;
using System;

public partial class SceneManager : Node
{
	[Export]
	public PackedScene gamescene;
	[Export]
	public Dictionary scenes;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PackedScene scene = (PackedScene) GD.Load((string) scenes["gamescene"]);
		AddChild(scene.Instantiate());
		
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

	public void ChangeScene(string sceneName)
	{
		// removes current loaded scene
        foreach (var child in GetChildren())
        {
            RemoveChild(child);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}