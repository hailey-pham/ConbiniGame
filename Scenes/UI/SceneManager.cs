using Godot;
using Godot.Collections;
using System;

public partial class SceneManager : Node
{
    [Export]
    public Dictionary scenes;

    [Signal]
    public delegate void SceneChangedEventHandler(string SceneName);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //load the main menu when the game starts
        PackedScene scene = (PackedScene)GD.Load((string)scenes["mainmenu"]);
        AddChild(scene.Instantiate());

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

    public void ChangeScene(string sceneName)
    {
        // removes current loaded scene
        foreach (var child in GetChildren())
        {
            RemoveChild(child);
        }

        try
        {
            PackedScene scene = (PackedScene)GD.Load((string)scenes[sceneName]);
            AddChild(scene.Instantiate());
            //announce what scene we've changed to
            //calendar uses this
            EmitSignal(nameof(SceneChanged), sceneName);
        }
        catch (Exception)
        {

            GD.PrintErr("Scene not found!");
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
