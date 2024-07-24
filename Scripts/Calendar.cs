using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml.Schema;

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
    private const int dayLength = 2 * 60; // each day is two minutes long
    private const int seasonLength = 3;

    private int currentDay = 1;
    private int currentSeason = 1;
    private string currentSeasonStr;
    private int currentCycle = 1;

    private Label timeLabel;
    private Label calendarLabel;
    private string[] seasons = { "Spring", "Summer", "Autumn", "Winter" };

    private SceneManager sceneManager;
    private globals globals;
    private disaster_stats stats;

    private Timer timer;

    private int[] weeklyDisasters;
    public int currentDayIndex = 0;
    public int nextDayIndex = 0;

    private int[] springArray;
    private int[] summerArray;
    private int[] autumnArray;
    private int[] winterArray;

    private bool WasPreviousDayDisaster = false;

    public static int DayLength => dayLength;

    [Export]
    private int[] firstSpringArray;
    private bool firstSpringPassed = false;

    private bool tutorialShowing = false;

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
        currentDay = 1;
        currentCycle = 1;
        // get scenemanager
        sceneManager = GetNode<SceneManager>("/root/SceneManager");
        sceneManager.SceneChanged += OnSceneChanged;

        // get the timer node
        timer = GetNode<Timer>("Timer");
        timer.Timeout += OnTimerTimeout;

        globals = GetNode<globals>("/root/Globals");

        currentSeasonStr = seasons[currentSeason - 1];
        GenerateDisasterCalendar();

        GD.Print("Day: " + currentDay + " Season: " + currentSeasonStr);
        GD.Print("Current day index: " + currentDayIndex);
        GD.Print("Next day index: " + nextDayIndex);

    }
    private void OnTimerTimeout()
    {
        elapsedTime += 1;

        int newPercent = elapsedTime * 100 / DayLength;

        if (newPercent != dayPercent)
        {
            dayPercent = newPercent;
            EmitSignal(SignalName.DayPercent, dayPercent);
        }

        if (elapsedTime > DayLength)
        {
            //dont apply time when we change scenes to the next day
            timer.Stop();
            elapsedTime = 0; // reset elapsed time

            if(npcSpawner.IsStoreEmpty())
            {
                EndDay();
            }
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
        if(currentDay == 3 && currentSeason == 4*currentCycle)
        {
            currentCycle += 1;
            sceneManager.ChangeScene("victory", "FadeToBlack");
        }

        if (IsNextDisasterDay() && ((DisasterType)nextDayIndex != DisasterType.Earthquake) && ((DisasterType)nextDayIndex != DisasterType.Tsunami))
        {
            sceneManager.ChangeScene("forecastscene", "FadeToBlack");
        }

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
        int totalSeasons = seasons.Length;
        currentSeason = newSeason;

        int currentSeasonIndex = (newSeason - 1) % totalSeasons;
        currentSeasonStr = seasons[currentSeasonIndex];

        GD.Print("Season has changed to: " + currentSeasonStr);
        UpdateCalendarLabel();
        if (newSeason > totalSeasons)
        {
            OnNewYear();
        }
    }

    private void OnSceneChanged(string sceneName)
    {
        if (sceneName == "gamescene")
        {
            // reset all daily stats
            globals.ResetDayStats();
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
            if (weeklyDisasters[seasonLength - 1] != (int)DisasterType.None)
            {
                WasPreviousDayDisaster = true;
            }
          
            currentDay = 1;
            currentSeason += 1;
            OnSeasonChange(currentSeason);
            UpdateCurrentWeekDisasters();
        }
        currentDayIndex = GetCurrentDayDisasterIndex();
        nextDayIndex = GetNextDayDisasterIndex();
        UpdateCalendarLabel();

        GD.Print("Day: " + currentDay + " Season: " + currentSeasonStr);
        GD.Print("Current day index: " + currentDayIndex);
        GD.Print("Next day index: " + nextDayIndex);

        if (!firstSpringPassed && !tutorialShowing)
        {
            ShowTutorial();
            tutorialShowing = true;
        }
        else if (firstSpringPassed)
        {
            tutorialShowing = false;
        }
    }

    private void ShowTutorial()
    {
        var sceneManager = GetNode<SceneManager>("/root/SceneManager");
        sceneManager.ChangeScene("tutorial");
    }

    public bool IsDisasterDay()
    {
        return (DisasterType)currentDayIndex != DisasterType.None;
    }

    public bool IsNextDisasterDay()
    {
        return (DisasterType)nextDayIndex != DisasterType.None;
    }

    //a silly check
    public bool IsDayOver()
    {
        return elapsedTime == 0;
    }

    public void DetermineNextDay()
    {
        GD.Print("Determining day...");
        var sceneManager = GetNode<SceneManager>("/root/SceneManager");

        if (currentDay == 1)
        {
            sceneManager.ChangeScene("seasontitle", "Sleep");
        }
        else if (IsDisasterDay())
        {
            sceneManager.ChangeScene("disasterscene","Sleep");
            // stats.UpdateMoney();
        }
        else
        {
            sceneManager.ChangeScene("gamescene","Sleep");
        }
    }

    private void OnNewYear()
    {
        // reset seasons after a year passes
        // idk how many years we want though
        currentSeason = 1;
        currentSeasonStr = seasons[0];
        currentDay = 1;

        GenerateDisasterCalendar();
        UpdateCurrentWeekDisasters();

        GD.Print("New Year Started: " + currentSeasonStr);
    }

    // disaster calendar generating things
    private int GetSeasonProbabilities(DisasterType disaster, string season)
    {
        // create different probabilities for each disaster depending on the current season
        int defaultProbability = 20;
        switch (disaster)
        {
            case DisasterType.Typhoon:
                if (season == "Summer" || season == "Autumn") return defaultProbability + 50;
                break;
            case DisasterType.WildFire:
                if (season == "Spring") return defaultProbability + 50;
                break;
            case DisasterType.FlashFlood:
                if (season == "Summer") return defaultProbability + 50;
                break;
            case DisasterType.HeavySnow:
                if (season == "Winter") return defaultProbability + 50;
                break;
            // earthquake and tsunami probability always stays the same
        }
        return defaultProbability;
    }

    private DisasterType AssignRandomDisaster(Random random, string season)
    {
        List<(DisasterType, int)> disasterTypes = new List<(DisasterType, int)>
        {
            (DisasterType.Earthquake, 20),
            (DisasterType.Tsunami, 20),
            (DisasterType.Typhoon, GetSeasonProbabilities(DisasterType.Typhoon, season)),
            (DisasterType.FlashFlood, GetSeasonProbabilities(DisasterType.FlashFlood, season)),
            (DisasterType.WildFire, GetSeasonProbabilities(DisasterType.WildFire, season)),
            (DisasterType.HeavySnow, GetSeasonProbabilities(DisasterType.HeavySnow, season)),
        };

        int totalProbability = 0;
        List<int> cumulativeProbabilities = new List<int>();

        foreach (var (disaster, probability) in disasterTypes)
        {
            // iterate through each tuple and add the disaster type's probability to the new list
            totalProbability += probability;
            cumulativeProbabilities.Add(totalProbability);
        }

        int randomValue = random.Next(totalProbability);

        for (int i = 0; i < cumulativeProbabilities.Count; i++)
        {
            if (randomValue < cumulativeProbabilities[i])
            {
                // find the first probability that is greater than the generated random value
                // assign disaster
                return disasterTypes[i].Item1;
            }
        }

        return DisasterType.None;
    }

    private int[] GenerateDisasterDaysSynchronous(string season)
    {
        int[] weeklyDisasters = new int[seasonLength];

        Random random = new Random();

        int randIndex = random.Next(1, seasonLength); // choose a random day, except first

        // assign a random disaster to the chosen day
        weeklyDisasters[randIndex] = (int)AssignRandomDisaster(random, season);

        // assign 0 (no disaster) to the rest of the days
        for (int i = 0; i < weeklyDisasters.Length; i++)
        {
            if (weeklyDisasters[i] == 0)
            {
                weeklyDisasters[i] = (int)DisasterType.None;
            }
        }

        return weeklyDisasters;
    }
    private async Task<int[]> GenerateDisasterDays(string season)
    {
        int[] weeklyDisasters = new int[seasonLength];

        Random random = new Random();

        Task shuffleTask = Task.Run(() =>
        {
            int randIndex = random.Next(1, seasonLength); // choose a random day, except first

            // assign a random disaster to the chosen day
            weeklyDisasters[randIndex] = (int)AssignRandomDisaster(random, season);

            // assign 0 (no disaster) to the rest of the days
            for (int i = 0; i < weeklyDisasters.Length; i++)
            {
                if (weeklyDisasters[i] == 0)
                {
                    weeklyDisasters[i] = (int)DisasterType.None;
                }
            }
        });
        return await Task.FromResult(weeklyDisasters);
    }
    public void GenerateDisasterCalendar()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        if (!firstSpringPassed && firstSpringArray != null && firstSpringArray.Length > 0)
        {
            springArray = firstSpringArray;
            firstSpringPassed = true;
        }
        else
        {
            springArray = GenerateDisasterDaysSynchronous("Spring");
        }
        // generate and output all the disaster arrays for the year at once

        summerArray = GenerateDisasterDaysSynchronous("Summer");
        autumnArray = GenerateDisasterDaysSynchronous("Autumn");
        winterArray = GenerateDisasterDaysSynchronous("Winter");

        GD.Print("Spring events: " + string.Join(",", springArray));
        GD.Print("Summer events: " + string.Join(",", summerArray));
        GD.Print("Autumn events: " + string.Join(",", autumnArray));
        GD.Print("Winter events: " + string.Join(",", winterArray));

        UpdateCurrentWeekDisasters();
        stopwatch.Stop();
        GD.Print($"Time to generate disaster calendar in ms: {stopwatch.ElapsedMilliseconds}");
    }


    private void UpdateCurrentWeekDisasters()
    {
        switch (currentSeasonStr)
        {
            case "Spring":
                weeklyDisasters = springArray;
                break;
            case "Summer":
                weeklyDisasters = summerArray;
                break;
            case "Autumn":
                weeklyDisasters = autumnArray;
                break;
            case "Winter":
                weeklyDisasters = winterArray;
                break;
            default:
                weeklyDisasters = new int[seasonLength];
                break;
        }
    }

    //getters
    public int GetCurrentSeason()
    {
        return currentSeason;
    }

    private int[] GetCurrentSeasonArray()
    {
        switch (currentSeasonStr)
        {
            case "Spring":
                return springArray;
            case "Summer":
                return summerArray;
            case "Autumn":
                return autumnArray;
            case "Winter":
                return winterArray;
            default:
                return new int[seasonLength];
        }
    }

    public int GetNextDayDisasterIndex()
    {
        int nextDay = (currentDay % seasonLength);
        int[] seasonArray = GetCurrentSeasonArray();
        int disasterIndex = seasonArray[nextDay];

        //if there is no disaster, earthquake, or tsunami...
        if(disasterIndex <= 2)
        {
            //report no disaster
            return 0;
        }

        return disasterIndex;
    }

    public int GetCurrentDayDisasterIndex()
    {
        int today = (currentDay - 1) % seasonLength;
        int[] seasonArray = GetCurrentSeasonArray();
        return seasonArray[today];
    }
    
    public DisastersEnum GetCurrentDayDisaster()
    {
        return (DisastersEnum) GetCurrentDayDisasterIndex();
    }

    public DisastersEnum GetNextDayDisaster()
    {
        return (DisastersEnum)GetNextDayDisasterIndex();
    }

    public int CurrentDay { get => currentDay; set => currentDay = value; }
    
    // debuggy
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
}