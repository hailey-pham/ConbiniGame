using Godot;
using System;
using System.Collections.Generic;

public partial class ItemSpawner : Node2D
{
    [Export]
    public PackedScene ItemScene;

    List<Node> currContains = new List<Node>();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

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
            currContains.Add(ItemBox);
        }
    }
    public void signal_inform_spawn()
    {
        this.spawn();
    }
    public void signal_delete()
    {
        foreach (Node a in currContains)
        {
            a.QueueFree();
        }
        //currContains[0].QueueFree();
        currContains.Clear();

        //recommended code to remove parent's child :3
        //Node childnode = GetChild(0);
        //if (childnode.GetParent() != null)
        //{
         //   childnode.GetParent().RemoveChild(childnode);
        //}
    }
}
