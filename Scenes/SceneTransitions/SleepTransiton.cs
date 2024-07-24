using Godot;
using System;
using System.Threading.Tasks;

public partial class SleepTransiton : SceneTransition
{
    [Export]
    private int numberOfZs = 3;

    private AnimationPlayer animationPlayer;
    [Export]
    private string fadeToBlackTransitionPath;

    private PackedScene fadeToBlackTransition;
    private FadeToBlackTransition fadeInstance;

    private Control spacebarIcon;
    private Control spacebarText;

    [Export] private int skipTimer = 20;

    private bool doubleTap = false;
    
    public override void _Ready()
    {
        spacebarText = GetNode<Control>("Label");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        fadeToBlackTransition = GD.Load<PackedScene>(fadeToBlackTransitionPath);
        fadeInstance = (FadeToBlackTransition) fadeToBlackTransition.Instantiate();
        
        HideChildren();
    }
    public override async Task PlayOutAsync()
    {
        AddChild(fadeInstance);
        await fadeInstance.PlayOutAsync();
        RemoveChild(fadeInstance);
        ShowChildren();

        for (int i = 0; i < numberOfZs; i++)
        {
            animationPlayer.Play("Z");
            await ToSignal(animationPlayer, "animation_finished");
        }

    }
    public override async Task PlayInAsync()
    {
        HideChildren();
        AddChild(fadeInstance);
        var fadeInstanceScript = fadeInstance;
        await fadeInstanceScript.PlayInAsync();
        RemoveChild(fadeInstance);        
    }
    
    private void HideChildren()
    {
        foreach(var child in GetChildren())
        {
            if(child is CanvasItem)
            {
                var canvasItemChild = (CanvasItem) child;
                canvasItemChild.Hide();
            }
        }
    }

    private void ShowChildren()
    {
        foreach (var child in GetChildren())
        {
            if (child is CanvasItem)
            {
                var canvasItemChild = (CanvasItem)child;
                canvasItemChild.Show();
            }
        }
    }
    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if(@event.IsAction("Interact"))
        {
            if (@event.IsPressed())
            {
                animationPlayer.SpeedScale = 4;
                Tween spacebarTween;
                spacebarTween = spacebarText.CreateTween();
                spacebarTween.TweenProperty(spacebarText, "modulate", new Color("ffffff00"), 0.5f);
                
                if(doubleTap) {
                    animationPlayer.Play("Z");
                    animationPlayer.Advance(2.5);
                }
            }
            else if(@event.IsReleased()){
                doubleTap = true;
            }
            else
            {
                animationPlayer.SpeedScale = 2;
            }
            
        }

    }
}
