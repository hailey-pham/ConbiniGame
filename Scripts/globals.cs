using Godot;
using System;

public partial class globals : Node
{
	private int _money;

	public int Money
	{
		get { return _money; }
		set { _money = value; }
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
