using Godot;
using System;

public partial class ProgressBar : TextureRect
{
    private AnimationPlayer animPlayer;
    public override void _Ready()
    {
        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        //animation is one minute by default, adjust speed to match day length
        animPlayer.SpeedScale = animPlayer.GetAnimation("dayProgress").Length / Calendar.DayLength;
        animPlayer.Play("dayProgress");
        
    }
}
