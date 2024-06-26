using Godot;
using System;

public partial class disaster : Control
{
	[Signal]
	public delegate void DisasterEndedEventHandler();

	private Button nextButton;
	private Timer displayTimer;

	private SceneManager sceneManager;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        // get scenemanager
        sceneManager = GetNode<SceneManager>("/root/SceneManager");
        
		nextButton = GetNode<Button>("NextButton");
		nextButton.Visible = false;
		nextButton.Pressed += OnNextButtonPressed;

		displayTimer = GetNode<Timer>("DisplayTimer");
		displayTimer.Timeout += OnDisplayTimerTimeout;
		displayTimer.Start();
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
