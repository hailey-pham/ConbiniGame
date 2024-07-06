using Godot;
using System;
using System.Linq;

public partial class CounterArea : Area2D
{
    public player Player = null;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    public void OnBodyEntered(Node2D body)
    {
        Player = body as player;
    }

    public void OnBodyExited(Node2D body)
    {
        if (body.Equals(Player))
        {
            Player = null;
        }
    }

    public bool PlayerIsInside()
    {
        if (Player != null)
        {
            return true;
        }
        return false;
    }
}
