using Godot;
using System;

public partial class Calendar : Node2D
{
    [Signal]
    public delegate void DayChangedEventHandler();

    [Signal]
    public delegate void DayPercentEventHandler(int percent);

    private int dayPercent = 0;

    private int elapsedTime = 0;
    private const int dayLength = 30; // 2 seconds long for testing purposes, change to 10 * 60 for the actual game
    private const int seasonLength = 7; // 7 days per season, or maybe 5?

    private int currentDay = 1;
    private int currentSeason = 1;
    private string currentSeasonStr;

    private Label timeLabel;
    private Label calendarLabel;
    private string[] seasons = { "Spring", "Summer", "Autumn", "Winter" };

    private SceneManager sceneManager;

    private Timer timer;

    public override void _Ready()
    {
        // get scenemanager
        sceneManager = GetNode<SceneManager>("/root/SceneManager");
        sceneManager.SceneChanged += OnSceneChanged;

        // get the timer node
        timer = GetNode<Timer>("Timer");
        timer.Timeout += OnTimerTimeout;

        // get the label nodes
        timeLabel = GetNode<Label>("TimeLabel");
        calendarLabel = GetNode<Label>("CalendarLabel");

        CustomizeLabels();

        OnSeasonChange(currentSeason);
        UpdateCalendarLabel();
    }

    private void OnTimerTimeout()
    {
        elapsedTime += 1;

        int newPercent = elapsedTime * 100/dayLength;

        if(newPercent != dayPercent)
        {
            dayPercent = newPercent;
            EmitSignal(SignalName.DayPercent, dayPercent);
        }

        if (elapsedTime > dayLength)
        {
            EmitSignal(nameof(DayChanged));
            //dont apply time when we change scenes to the next day
            timer.Stop();

            elapsedTime = 0; // reset elapsed time
            currentDay += 1;
            if (currentDay > seasonLength)
            {
                currentDay = 1;
                currentSeason += 1;
                OnSeasonChange(currentSeason);
            }
            else
            {
                UpdateCalendarLabel();
                sceneManager.ChangeScene("endofdayscene");
            }
        }
        else
        {
            int minutes = elapsedTime / 60;
            int seconds = elapsedTime % 60;
            timeLabel.Text = string.Format("Elapsed Time: {0:D2}:{1:D2}", minutes, seconds);
        }
    }

    private void UpdateCalendarLabel()
    {
        calendarLabel.Text = string.Format("Season: {0}, Day: {1}", currentSeasonStr, currentDay);
    }

    private void OnSeasonChange(int newSeason)
    {
        // print and loop through seasons
        int totalSeasons = 4;

        int currentSeasonIndex = (newSeason - 1) % totalSeasons;
        currentSeasonStr = seasons[currentSeasonIndex];

        GD.Print("Season has changed to: " + currentSeasonStr);
        UpdateCalendarLabel();
    }

    private void OnSceneChanged(string sceneName)
    {
        if(sceneName == "gamescene")
        {
            timer.Start();
        }
    }

    private void CustomizeLabels()
    {
        // in case we want to customize labels further
    }
}
