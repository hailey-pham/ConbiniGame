using Godot;
using System;
using System.Collections.Generic;

public partial class disaster : Control
{
	[Signal]
	public delegate void DisasterEndedEventHandler();

	private Button nextButton;
	private Timer displayTimer;
	private WarningAnimation warningAnimation;

	private SceneManager sceneManager;
	private Calendar calendar;

	// Called when the node enters the scene tree for the first time

	[Export]
	private Calendar.DisastersEnum currDisaster;

	private globals globals;

	
	public override void _Ready()
	{
        // get scenemanager
        sceneManager = GetNode<SceneManager>("/root/SceneManager");
		warningAnimation = GetNode<WarningAnimation>("WarningAnimation");
		calendar = GetNode<Calendar>("/root/Calendar");
		globals = GetNode<globals>("/root/Globals");

		currDisaster = calendar.GetCurrentDayDisaster();

		//change disasters to not always be hardcoded to earthquakes
		warningAnimation.SetDisasterType(currDisaster);

        //load in the disasterAnimation that matches our current enum

        var disasterAnim = GD.Load<PackedScene>("res://Scenes/UI/DisasterAnimations/" + Enum.GetName(typeof(Calendar.DisastersEnum), currDisaster) + "Animation.tscn");
		AddChild(disasterAnim.Instantiate());

        nextButton = GetNode<Button>("NextButton");
		//we have to move button in front of animation to get it to register inputs
		MoveChild(nextButton, -1);
		nextButton.Visible = false;
		nextButton.Pressed += OnNextButtonPressed;

		displayTimer = GetNode<Timer>("DisplayTimer");
		displayTimer.Timeout += OnDisplayTimerTimeout;
		displayTimer.Start();

		foreach (KeyValuePair<string, Upgrade> upgrade in globals.Upgrades) {
            if (upgrade.Value.owned) {
                upgrade.Value.onDisaster(globals, currDisaster);
            }
        }
	}

	private void OnNextButtonPressed()
	{
		GD.Print("Next button pressed!");
		EmitSignal(nameof(DisasterEnded));
		sceneManager.ChangeScene("disasterstatsscene");
	}

	private void OnDisplayTimerTimeout()
	{
		nextButton.Visible = true;
	}

	private void OnDisasterEnded()
	{
		GD.Print("Transitioning to disaster stats...");
		sceneManager.ChangeScene("disasterstatsscene");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
