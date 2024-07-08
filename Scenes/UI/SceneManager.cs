using Godot;
using Godot.Collections;
using System;
using System.Threading.Tasks;

public partial class SceneManager : Node
{
    [Export]
    public Dictionary scenes;

    private AnimationPlayer animationPlayer;
    private Node sceneParent;

    [Signal]
    public delegate void SceneChangedEventHandler(string SceneName);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //load references to children components
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        sceneParent = GetNode<Node>("SceneParent");

        //load the main menu when the game starts
        LoadNewScene("mainmenu");

    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
        if (@event.IsAction("Exit"))
        {
            foreach (var child in GetChildren())
            {
                RemoveChild(child);
            }
        }
    }

    public async void ChangeScene(string sceneName)
    {
        animationPlayer.Play("fade_out");

        await ToSignal(animationPlayer, "animation_finished");

        RemoveChildScenes();
        LoadNewScene(sceneName);

        animationPlayer.Play("fade_in");
    }

    private void RemoveChildScenes()
    {
        foreach (var child in sceneParent.GetChildren())
        {
            sceneParent.RemoveChild(child);
        }
    }

    private void LoadNewScene(string sceneName)
    {
        try
        {
            PackedScene scene = (PackedScene)GD.Load((string)scenes[sceneName]);
            sceneParent.AddChild(scene.Instantiate());
            //announce what scene we've changed to
            //calendar uses this
            EmitSignal(nameof(SceneChanged), sceneName);
        }
        catch (Exception)
        {

            GD.PrintErr("Scene not found!");
        }
    }
}
