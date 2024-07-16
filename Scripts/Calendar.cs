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
    private const int dayLength = 1 * 60;
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
    public int currentDayIndex = 0;
    public int nextDayIndex = 0;

    private enum DisasterType
    {
        None = 0,
        Earthquake = 1,
        Tsunami = 2,
        Typhoon = 3,
        WildFire = 4,
        FlashFlood = 5,
        HeavySnow = 6
    }

    public enum DisastersEnum
    {
        None,
        Earthquake,
        Tsunami,
        Typhoon,
        WildFire,
        FlashFlood,
        HeavySnow,
    }

    public override void _Ready()
    {
        // get scenemanager
        sceneManager = GetNode<SceneManager>("/root/SceneManager");
        sceneManager.SceneChanged += OnSceneChanged;

        // get the timer node
        timer = GetNode<Timer>("Timer");
        timer.Timeout += OnTimerTimeout;

        globals = GetNode<globals>("/root/Globals");

        GenerateDisasterDays();

        // print calendar stuff
        GD.Print("This week's disasters: " + string.Join(",", weeklyDisasters));

        currentDayIndex = GetCurrentDayDisasterIndex();
        nextDayIndex = GetNextDayDisasterIndex();
        GD.Print("Current day index: " + currentDayIndex);
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
        sceneManager.ChangeScene("endofdayscene", "FadeToBlack");
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
        return weeklyDisasters[currentDayIndex] != (int)DisasterType.None;
    }

    //a silly check
    public bool IsDayOver()
    {
        return elapsedTime == 0;
    }

    public void DetermineNextDay()
    {
        GD.Print("Determining day...");
        currentDayIndex = GetCurrentDayDisasterIndex();
        nextDayIndex = GetNextDayDisasterIndex();
        GD.Print("Current day index: " + currentDayIndex);
        GD.Print("Next day index: " + nextDayIndex);
        var sceneManager = GetNode<SceneManager>("/root/SceneManager");

        if (IsDisasterDay())
        {
            sceneManager.ChangeScene("disasterscene","Sleep");
            // stats.UpdateMoney();
        }
        else
        {
            sceneManager.ChangeScene("gamescene","Sleep");
        }
    }

    // disaster calendar generating things

    // to do : make the disaster probability depend on the season
    public void GenerateDisasterDays()
    {
        int[] weeklyArray = { 0, 0, 0, 0, 0, 0, 0 };
        List<int> list = new List<int>(weeklyArray);
        weeklyDisasters = new int[7];
        List<int> disasterTypes = new List<int> { 1, 2, 3, 4, 5, 6 };

        Task shuffleTask = Task.Run(() =>
        {
            Random random = new Random();
            int disasterCount = 0;

            // assign two disasters to random days
            while (disasterCount < 2)
            {
                int randIndex;
                do
                {
                    randIndex = random.Next(1, 7); // make sure the first day is a normal day
                }
                while (weeklyDisasters[randIndex] != 0 || // make sure there's no disasters two days in a row
                       (randIndex > 0 && weeklyDisasters[randIndex - 1] != 0) ||
                       (randIndex < 6 && weeklyDisasters[randIndex + 1] != 0));

                // assign random disaster to the disaster days
                int disasterTypeIndex = random.Next(disasterTypes.Count);
                weeklyDisasters[randIndex] = disasterTypes[disasterTypeIndex];
                disasterTypes.RemoveAt(disasterTypeIndex);

                disasterCount++;
            }

            // assign 0 to the rest of the days
            for (int i = 0; i < weeklyArray.Length; i++)
            {
                if (weeklyDisasters[i] == 0)
                {
                    weeklyDisasters[i] = (int)DisasterType.None;
                }
            }
        });

        shuffleTask.Wait();
    }

    //getters
    public int GetCurrentSeason()
    {
        return currentSeason;
    }

    public int GetNextDayDisasterIndex()
    {
        int nextDay = (currentDay % 7);
        return weeklyDisasters[nextDay];
    }

    public int GetCurrentDayDisasterIndex()
    {
        int today = (currentDay - 1) % 7;
        return weeklyDisasters[today];
    }
    
    public override void _UnhandledKeyInput(InputEvent @event)
    {
        //only allow for debug key inputs if in debug mode
#if DEBUG
        if (@event.IsActionPressed("DEBUG_skip_day"))
        {
            timer.Stop();
            elapsedTime = 0; // reset elapsed time
            EndDay();
        }
#endif
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