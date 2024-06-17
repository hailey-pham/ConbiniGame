using Godot;
using System;

public partial class Box : Area2D
{

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.BodyEntered += _on_body_entered;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void _on_player_hit()
    {
		GD.Print("AAAAAAA :3");
    }
	public void _on_body_entered(Node2D body)
	{
		 if (Input.IsActionPressed("Interact")) {
			Hide();
		}

		GD.Print("Life is パン");
	}
}
