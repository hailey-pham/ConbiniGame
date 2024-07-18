using Godot;
using System;

public partial class forecast : Control
{
	[Signal]
	public delegate void ForecastEndedEventHandler();

    private Timer displayTimer;
    private ForecastAnimation forecastAnimation;

    private SceneManager sceneManager;
    private Calendar calendar;
    private globals globals;

    [Export]
    private Calendar.DisastersEnum nextDisaster;
    public override void _Ready()
	{
        // get scenemanager
        sceneManager = GetNode<SceneManager>("/root/SceneManager");
        forecastAnimation = GetNode<ForecastAnimation>("ForecastAnimation");
        calendar = GetNode<Calendar>("/root/Calendar");
        globals = GetNode<globals>("/root/Globals");

        nextDisaster = calendar.GetNextDayDisaster();

        forecastAnimation.SetDisasterType(nextDisaster);

        var forecastAnim = GD.Load<PackedScene>("res://Scenes/UI/ForecastAnimation.tscn");
        AddChild(forecastAnim.Instantiate());

        displayTimer = GetNode<Timer>("DisplayTimer");
        displayTimer.Timeout += OnDisplayTimerTimeout;
        displayTimer.Start();
    }

    public void OnDisplayTimerTimeout()
    {
        displayTimer.Stop();
        sceneManager.ChangeScene("gamescene");
    }
}
