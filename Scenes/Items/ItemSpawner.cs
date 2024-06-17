using Godot;
using System;

public partial class ItemSpawner : Node2D
{
    [Export]
    public PackedScene ItemScene;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        spawn();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void spawn()
    {
        if (ItemScene != null)
        {
            Node ItemBox = ItemScene.Instantiate();
            //ItemBox.Set("z_as_relative", false);
            //newBox.Set("Scale", 0.5);
            AddChild(ItemBox);
        }
    }
}
