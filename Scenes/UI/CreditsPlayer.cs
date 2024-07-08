using Godot;
using System;

public partial class CreditsPlayer : AnimationPlayer
{
    public override void _Ready()
    {
        Play("scroll");
    }
}
