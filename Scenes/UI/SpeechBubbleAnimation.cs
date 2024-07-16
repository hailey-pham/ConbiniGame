using Godot;
using System;

public partial class SpeechBubbleAnimation : Node2D
{
    private AnimationPlayer animationPlayer;
    private Timer timer;

    public override void _Ready()
    {
        //play the initial bubble animation when added to scene
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animationPlayer.Play("bubble");

        //start the timer which will remove the anim
        timer = GetNode<Timer>("Timer");
        timer.Timeout += OnTimerTimeout;
        timer.Start();
    }

    private async void OnTimerTimeout()
    {
        animationPlayer.PlayBackwards("bubble");
        await ToSignal(animationPlayer, "animation_finished");
        QueueFree();
    }
}
