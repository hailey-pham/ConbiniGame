using Godot;
using System;

public partial class TsunamiAnimation : Control
{
    private AnimationPlayer animPlayer;

    public override void _Ready()
    {
        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animPlayer.Play("waves");
    }
}
