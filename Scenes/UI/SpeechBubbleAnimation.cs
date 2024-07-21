using ConbiniGame.Scripts;
using Godot;
using System;
using System.Diagnostics;
using Debug = ConbiniGame.Scripts.Debug;

public partial class SpeechBubbleAnimation : Node2D
{
    private AnimationPlayer animationPlayer;
    private Timer timer;
    private Sprite2D itemSprite;
    private ItemRes item;

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
        item = NPCPreferenceModifier.GetAPopularItem();
        itemSprite.Texture = item.spriteTexture;
    }
    
    
    public ItemRes GetItem()
    {
        //since the item is initialized after the scene is added to the tree, only call this method after we've entered tree
        Debug.Assert(IsInsideTree(),"Accessed speech bubble item before entering frame");
        return item;
    }
    private async void OnTimerTimeout()
    {
        animationPlayer.PlayBackwards("bubble");
        await ToSignal(animationPlayer, "animation_finished");
        QueueFree();
    }
}
