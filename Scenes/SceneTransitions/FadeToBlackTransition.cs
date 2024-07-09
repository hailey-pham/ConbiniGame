using Godot;
using System;
using System.Threading.Tasks;

public partial class FadeToBlackTransition : SceneTransition
{
    private AnimationPlayer animationPlayer;
    public override void _Ready()
    {
       animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }
    public override async Task PlayOutAsync()
    {
        //do the transitioning
        animationPlayer.Play("fade_out");
        await ToSignal(animationPlayer, "animation_finished");
    }

    public override async Task PlayInAsync()
    {
        animationPlayer.Play("fade_in");
        await ToSignal(animationPlayer, "animation_finished");
    }
}
