using Godot;
using System;
using System.Linq;

public partial class counter_area : Area2D
{
    bool hasObject;
    player Player = null;
    [Signal]
    public delegate void InformSpawnEventHandler();
    [Signal]
    public delegate void DeleteEventHandler();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        hasObject = false;
    }


    public void OnBodyEntered(Node2D body)
    {
        Player = body as player;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        var interact = false;
        if (Input.IsActionJustPressed("Interact"))
        {
            interact = true;
        }
        Node bodyChecker = null;

        foreach (var bodies in GetOverlappingBodies()) {
            if (bodies.Name == "Player")
            {
                bodyChecker = bodies;
                //bodyChecker.GetNode(bodies.GetPath());
            }
        }

        //This is messy but I just want the player to interact :3
        //if interact button has been pressed and the overlapping body is the player, 
        //Test: Player starts with an item and can only place on of them on any counter
        if (interact && bodyChecker != null)
         {
            GD.Print(hasObject);
            if (hasObject == true && Player.playerHasObject == false)
            {
                Player.playerHasObject = true;
                hasObject = false;
                EmitSignal("Delete");
            }
            else if (hasObject == false && Player.playerHasObject == true)
            {
                Player.playerHasObject = false;
                hasObject = true;
                EmitSignal("InformSpawn");
            }
            
        }
    }
}
