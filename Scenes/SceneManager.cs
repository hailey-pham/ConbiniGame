using Godot;
using System;

public partial class SceneManager : Node
{
	[Export]
	public PackedScene gamescene;
	// Called when the node enters the scene tree for the first time.

	[Export]
	public PackedScene endOfDayScene;


	private Node currentScene;

    /* public override void _Ready()
	{

		var scene = gamescene.Instantiate();
		AddChild(scene);
		
	} */

    public override void _Ready()
    {
        SwitchToScene(gamescene);

        if (endOfDayScene == null)
        {
            GD.PrintErr("endOfDayScene is not assigned! Assigning now...");
            endOfDayScene = (PackedScene)ResourceLoader.Load("res://Scenes/UI/end_of_day_menu.tscn");
        }

    }

    private void OnUpgradeButtonPressed()
    {
        GD.Print("Upgrade button pressed in SceneManager!");

        // Remove the current EndOfDay menu
        foreach (Node node in GetTree().GetNodesInGroup("EndOfDayMenu"))
        {
            node.QueueFree();
        }

        // Switch back to the world scene
        SwitchToScene(gamescene);
    }

    private void OnSleepButtonPressed()
    {
        GD.Print("Sleep button pressed in SceneManager!");

        // Remove the current EndOfDay menu
        foreach (Node node in GetTree().GetNodesInGroup("EndOfDayMenu"))
        {
            node.QueueFree();
        }

        // Switch back to the world scene
        SwitchToScene(gamescene);
    }

    private void SwitchToScene(PackedScene newScene)
    {
        if (currentScene != null)
        {
            RemoveChild(currentScene);
            currentScene.QueueFree();
        }

        currentScene = newScene.Instantiate();
        AddChild(currentScene);
    }

    public void ShowEndOfDayMenu()
	{
        var endOfDayMenu = (Control)endOfDayScene.Instantiate();

        AddChild(endOfDayMenu);

        endOfDayMenu.Connect(nameof(EndOfDay.UpgradeButtonPressed), new Callable(this, nameof(OnEndOfDayMenuClosed)));
        endOfDayMenu.Connect(nameof(EndOfDay.SleepButtonPressed), new Callable(this, nameof(OnEndOfDayMenuClosed)));
    }

    private void OnEndOfDayMenuClosed()
    {
        GD.Print("End of day menu closed!");
        var scene = gamescene.Instantiate();
        AddChild(scene);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
        if (@event.IsAction("Exit"))
        {
            if (currentScene != null)
            {
                RemoveChild(currentScene);
                currentScene.QueueFree();
                currentScene = null;
            }
        }
    }

    /* public override void _UnhandledInput(InputEvent @event)
     {
         base._UnhandledInput(@event);
         if (@event.IsAction("Exit"))
         {
             foreach (var child in GetChildren())
             {
                 RemoveChild(child);
             }
         }
     } */
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
 
}
