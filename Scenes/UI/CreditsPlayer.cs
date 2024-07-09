using Godot;
using System;

public partial class CreditsPlayer : AnimationPlayer
{
    [Export]
    public float speedUp = 2f;
    public override void _Ready()
    {
        Play("scroll");
        AnimationFinished += OnAnimationFinished;
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event.IsActionPressed("Interact"))
        {
            SpeedScale = speedUp;
        }

        if (@event.IsActionReleased("Interact"))
        {
            SpeedScale = 1f;
        }
    }

    private void OnAnimationFinished(StringName name)
    {
        var sceneManager = GetNode<SceneManager>("/root/SceneManager");
        sceneManager.ChangeScene("mainmenu","FadeToBlack");
    }
}
