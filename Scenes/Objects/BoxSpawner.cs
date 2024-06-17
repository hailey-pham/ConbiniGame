using Godot;
using System;

public partial class BoxSpawner : Node2D
{
    [Export]
    public PackedScene BoxScene;

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
        if (BoxScene != null)
        {
            Node newBox = BoxScene.Instantiate();
            newBox.Set("z_as_relative", false);
            //newBox.Set("Scale", 0.5);
            AddChild(newBox);
        }
    }
}

