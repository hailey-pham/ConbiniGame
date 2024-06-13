using Godot;
using System;

public partial class Calendar : Node2D
{
    private int elapsedTime = 0;
    private const int dayLength = 2; // change to 10 * 60 for the actual game
    private const int seasonLength = 7; // 7 days per season

    private int currentDay = 1;
    private int currentSeason = 1;

    private Label timeLabel;
    private Label calendarLabel;

    public override void _Ready()
    {
        // Get the Timer node
        Timer timer = GetNode<Timer>("Timer");
        timer.Timeout += OnTimerTimeout;
        timer.Start();

        // Get the Label nodes
        timeLabel = GetNode<Label>("TimeLabel");
        calendarLabel = GetNode<Label>("CalendarLabel");

        CustomizeLabels();

        UpdateCalendarLabel();
    }

    private void OnTimerTimeout()
    {
        elapsedTime += 1;
        if (elapsedTime > dayLength)
        {
            elapsedTime = 0; // Reset elapsed time
            currentDay += 1;
            if (currentDay > seasonLength)
            {
                currentDay = 1;
                currentSeason += 1;
                OnSeasonChange(currentSeason);
            }
            UpdateCalendarLabel();
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
        calendarLabel.Text = string.Format("Season: {0}, Day: {1}", currentSeason, currentDay);
    }

    private void OnSeasonChange(int newSeason)
    {
        GD.Print("Season has changed to: " + newSeason);
        switch (newSeason)
        {
            case 1:
                GD.Print("It's Spring!");
                break;
            case 2:
                GD.Print("It's Summer!");
                break;
            case 3:
                GD.Print("It's Autumn!");
                break;
            case 4:
                GD.Print("It's Winter!");
                break;
            default:
                GD.Print("New Season: " + newSeason);
                break;
        }
    }

    private void CustomizeLabels()
    {

    }
}