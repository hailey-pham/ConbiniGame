using Godot;
using System;

public partial class SpeechBubbleAnimation : Node2D
{
    private AnimationPlayer animationPlayer;
    private Timer timer;
    private Sprite2D itemSprite;

    public override void _Ready()
    {
        //get reference to the sprite inside the bubble
        itemSprite = GetNode<Sprite2D>("Bubble/ItemSprite");

        //play the initial bubble animation when added to scene
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animationPlayer.Play("bubble");

        //start the timer which will remove the anim
        timer = GetNode<Timer>("Timer");
        timer.Timeout += OnTimerTimeout;
        timer.Start();

        //set the bubble item texture
        itemSprite.Texture = NPCPreferenceModifier.GetAPopularItem().spriteTexture;
    }

    private async void OnTimerTimeout()
    {
        animationPlayer.PlayBackwards("bubble");
        await ToSignal(animationPlayer, "animation_finished");
        QueueFree();
    }
}
