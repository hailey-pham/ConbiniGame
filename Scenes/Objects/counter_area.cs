using Godot;
using System;
using System.Linq;

public partial class counter_area : Area2D
{

    [Signal]
    public delegate void InformSpawnEventHandler();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        var interact = false;
        if (Input.IsActionJustPressed("Interact"))
        {
            interact = true;
        }
        Node2D bodyChecker = null;

        foreach (var bodies in GetOverlappingBodies()) {
            if (bodies.Name == "Player")
            {
                bodyChecker = bodies;
            }
        }

        //This is messy but I just want the player to interact :3
        if (interact && bodyChecker != null)
        {
            EmitSignal("InformSpawn");
        }
    }

    public void _on_counter_transfer_signal()
    {
        
    }
}
