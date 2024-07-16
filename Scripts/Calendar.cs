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
    private const int seasonLength = 4;

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

    private int[] weeklyDisasters;
    public int currentDayIndex = 0;
    public int nextDayIndex = 0;

    private int[] springArray;
    private int[] summerArray;
    private int[] autumnArray;
    private int[] winterArray;

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
        // get scenemanager
        sceneManager = GetNode<SceneManager>("/root/SceneManager");
        sceneManager.SceneChanged += OnSceneChanged;

        // get the timer node
        timer = GetNode<Timer>("Timer");
        timer.Timeout += OnTimerTimeout;

        globals = GetNode<globals>("/root/Globals");

        GenerateDisasterCalendar();

        UpdateCurrentWeekDisasters();

        GD.Print("Day: " + currentDay + " Season: " + currentSeasonStr);
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
            UpdateCurrentWeekDisasters();
        }
        currentDayIndex = GetCurrentDayDisasterIndex();
        nextDayIndex = GetNextDayDisasterIndex();
        UpdateCalendarLabel();

        GD.Print("Day: " + currentDay + " Season: " + currentSeasonStr);
        GD.Print("Current day index: " + currentDayIndex);
        GD.Print("Next day index: " + nextDayIndex);
    }

    public bool IsDisasterDay()
    {
        if (currentDayIndex < 0 || currentDayIndex >= weeklyDisasters.Length)
        {
            GD.PrintErr("IsDisasterDay: Index out of bounds. Current Day Index: " + currentDayIndex);
            return false;
        }
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
        int defaultProbability = 0;
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

    public async void GenerateDisasterCalendar()
    {
        // generate and output all the disaster arrays at once
        var springTask = Task.Run(() => GenerateDisasterDays("Spring"));
        var summerTask = Task.Run(() => GenerateDisasterDays("Summer"));
        var autumnTask = Task.Run(() => GenerateDisasterDays("Autumn"));
        var winterTask = Task.Run(() => GenerateDisasterDays("Winter"));

        springArray = await springTask;
        summerArray = await summerTask;
        autumnArray = await autumnTask;
        winterArray = await winterTask;

        GD.Print("Spring events: " + string.Join(",", springArray));
        GD.Print("Summer events: " + string.Join(",", summerArray));
        GD.Print("Autumn events: " + string.Join(",", autumnArray));
        GD.Print("Winter events: " + string.Join(",", winterArray));

        UpdateCurrentWeekDisasters();
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
        return seasonArray[nextDay];
    }

    public int GetCurrentDayDisasterIndex()
    {
        int today = (currentDay - 1) % seasonLength;
        int[] seasonArray = GetCurrentSeasonArray();
        return seasonArray[today];
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