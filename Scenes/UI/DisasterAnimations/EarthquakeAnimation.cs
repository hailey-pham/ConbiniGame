using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class EarthquakeAnimation : Control
{

	private AnimationPlayer animPlayer;
	private TextureRect bg;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		bg = GetNode<TextureRect>("Background");
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animPlayer.Play("shake");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
