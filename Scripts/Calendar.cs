using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Calendar : Node2D
{
    [Signal]
    public delegate void DayChangedEventHandler();

    [Signal]
    public delegate void DayPercentEventHandler(int percent);

    [Signal]
    public delegate void DisplayEndOfDayStatsEventHandler();

    private int dayPercent = 0;

    private int elapsedTime = 0;
    private const int dayLength = 1 * 60; // 2 seconds long for testing purposes, change to 10 * 60 for the actual game
    private const int seasonLength = 7; // 7 days per season, or maybe 5?

    private int currentDay = 1;
    private int currentSeason = 1;
    private string currentSeasonStr;

    private Label timeLabel;
    private Label calendarLabel;
    private string[] seasons = { "Spring", "Summer", "Autumn", "Winter" };

    private SceneManager sceneManager;
    private globals globals;
    private disaster_stats stats;

    private Timer timer;

    private bool disasterDay = false;
    private int[] weeklyDisasters;
    public int currentDayIndex;
    public int nextDayIndex;

    public override void _Ready()
    {
        // get scenemanager
        sceneManager = GetNode<SceneManager>("/root/SceneManager");
        sceneManager.SceneChanged += OnSceneChanged;

        // get the timer node
        timer = GetNode<Timer>("Timer");
        timer.Timeout += OnTimerTimeout;

        globals = GetNode<globals>("/root/Globals");

        GenerateDisasterCalendar();

        GD.Print("Weekly events: " + string.Join(",", weeklyDisasters));

        GetCurrentDayDisasterIndex();
        GetNextDayDisasterIndex();
        GD.Print("Current day index:" + currentDayIndex);
        GD.Print("Next day index: " + nextDayIndex);
    }

    private void OnTimerTimeout()
    {
        elapsedTime += 1;

        int newPercent = elapsedTime * 100 / dayLength;

        if (newPercent != dayPercent)
        {
            dayPercent = newPercent;
            EmitSignal(SignalName.DayPercent, dayPercent);
        }

        if (elapsedTime > dayLength)
        {
            //dont apply time when we change scenes to the next day
            timer.Stop();
            elapsedTime = 0; // reset elapsed time
        }
        else
        {
            int minutes = elapsedTime / 60;
            int seconds = elapsedTime % 60;
            timeLabel.Text = string.Format("Elapsed Time: {0:D2}:{1:D2}", minutes, seconds);
        }
    }

    //called from the npcSpawner once all npcs have left the store
    public void EndDay()
    {
        IncrementDay();
        sceneManager.ChangeScene("endofdayscene");
        EmitSignal(nameof(DisplayEndOfDayStats));
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
        if (sceneName == "gamescene")
        {
            // reset earnings and customers at the start of the new day
            globals.ResetEarnings();
            globals.ResetCustomers();
            timer.Start();
            // get the label nodes
            timeLabel = GetNode<Label>("/root/SceneManager/SceneParent/World/UI/TimeLabel");
            calendarLabel = GetNode<Label>("/root/SceneManager/SceneParent/World/UI/CalendarLabel");
            UpdateCalendarLabel();
            OnSeasonChange(currentSeason);
        }

    }


    public void IncrementDay()
    {
        globals.IncrementDay();
        currentDay += 1;
        if (currentDay > seasonLength)
        {
            currentDay = 1;
            currentSeason += 1;
            OnSeasonChange(currentSeason);
        }
        UpdateCalendarLabel();
    }

    public bool IsDisasterDay()
    {
        // hard coding disaster day
        return currentDay == 2 || currentDay == 4;
    }

    //a silly check
    public bool IsDayOver()
    {
        return elapsedTime == 0;
    }

    public void DetermineNextDay()
    {
        GD.Print("Determining day...");
        GD.Print("Current Day: " + currentDay);
        var sceneManager = GetNode<SceneManager>("/root/SceneManager");

        if (IsDisasterDay())
        {
            sceneManager.ChangeScene("disasterscene");
            // stats.UpdateMoney();
        }
        else
        {
            sceneManager.ChangeScene("gamescene");
        }
    }

    //getters
    public int GetCurrentSeason()
    {
        return currentSeason;
    }

    public void GenerateDisasterCalendar()
    {
        int[] weeklyArray = { 0, 0, 0, 0, 0, 1, 1 };
        List<int> list = new List<int>(weeklyArray);
        weeklyDisasters = new int[7];

        Task shuffleTask = Task.Run(() =>
        {

            Random random = new Random();
            int lastDayDisaster = -1;
            int disasterCount = 0;

            for (int i = 0; i < weeklyArray.Length; i++)
            {
                if (i == 0)
                {
                    weeklyDisasters[i] = 0;
                    continue; // first day is always a normal day
                }

                int randIndex;
                int eventDay;

                do
                {
                    randIndex = random.Next(list.Count);
                    eventDay = list[randIndex];
                }
                while ((eventDay == 1 && lastDayDisaster == 1) || (i == 1 && eventDay == 1)); // make sure there isn't two consecutive disasters

                if (eventDay == 1)
                    {
                        disasterCount++;
                        if (disasterCount > 2)
                        {
                            eventDay = 0;
                        }
                    }
                weeklyDisasters[i] = eventDay;
                lastDayDisaster = eventDay;
                list.RemoveAt(randIndex);
                }
                while (disasterCount < 2)
                {
                    int indexToChange;
                    do
                    {
                        indexToChange = random.Next(1, 7);
                    }
                while (weeklyDisasters[indexToChange] == 1 ||
                     (indexToChange > 0 && weeklyDisasters[indexToChange - 1] == 1) ||
                     (indexToChange < 6 && weeklyDisasters[indexToChange + 1] == 1));

                    weeklyDisasters[indexToChange] = 1;
                    disasterCount++;
                }
                while (disasterCount > 2)
                {
                    int indexToChange;
                    do
                    {
                        indexToChange = random.Next(1, 7);
                    }
                    while (weeklyDisasters[indexToChange] == 0);

                    weeklyDisasters[indexToChange] = 0;
                    disasterCount--;
                }
        });
        shuffleTask.Wait();
     }

    public int GetNextDayDisasterIndex()
    {
        return weeklyDisasters[(currentDayIndex + 1) % weeklyDisasters.Length];
    }

    public int GetCurrentDayDisasterIndex()
    {
        return weeklyDisasters[currentDayIndex];
    }

    /*
     * 
     * MULTITHREADED FOR PERFORMANT ASPECT
     * 
     */

    //a method to return the disaster (or no disaster) occuring tomorrow

    //run this once before new week starts
    //array of possible events (0, 0, 0, 0, 0, 1, 1)
    //get a random integer range 0-array.count
    //get the value at array[rand] and put it in new array
    //newarray = (0)
    //remove value at array[rand]
    //repeat until original array is empty.

    //GetNextDayDisasterIndex -> int
    //a method for checking tomorrow's disaster

    //GetCurrentDayDisasterIndex -> int
    //a method for checking the current day's disaster
}