
using ConbiniGame.Scripts;
using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Timer = Godot.Timer;

public partial class tutorial : Control
{
    private RichTextLabel tutorialText;
    private int currentStepIndex = 0;
    private string currentTutorialName = "opening";

    private Timer tutorialTimer;

    [Export]
    private float textDelay = 0.05f;

    private bool isTyping = false;
    private int part = 0;

    private bool isTutorialPlaying = false;
    private bool isFirstCounterStocked = false;
    private bool isFirstTimeInventory = false;
    private bool isFirstTimeEndOfDay = false;
    private bool isFirstDay = true;

    private SceneManager sceneManager;
    private counter counter;
    private storage storage;
    private AudioStreamPlayer audioStreamPlayer;
    private AnimationPlayer animationPlayer;

    private Dictionary<string, DialogueLine[]> script = new();

    [Signal]
    public delegate void TutorialFinishedEventHandler();

    struct DialogueLine
    {
        public DialogueLine(string text, string animationName = "talking_concerned")
        {
            Text = text;
            AnimationName = animationName;
        }

        public string Text { get; }
        public string AnimationName { get; }
    }

    public override void _Ready()
    {
        //this must be called before the script is read from.
        BuildScript();

        Hide();
        sceneManager = GetNode<SceneManager>("/root/SceneManager");
        sceneManager.SceneChanged += OnSceneChanged;
        tutorialText = GetNode<RichTextLabel>("TutorialText");
        audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        tutorialTimer = new Timer();
        tutorialTimer.WaitTime = 1.0f;
        tutorialTimer.OneShot = true;
        tutorialTimer.Timeout += OnTutorialTimerTimeout;
        AddChild(tutorialTimer);

        ProcessMode = ProcessModeEnum.Always;
    }

    //adds all the text to our script dictionary
    private void BuildScript()
    {
        var opening = new DialogueLine[]
        {
            new DialogueLine("Grandpa passed away in the winter."),
            new DialogueLine("He owned this store..."),
            new DialogueLine("Apparently I'm the one to inherit it."),
            new DialogueLine("He lived in this village, prone to disasters. I'm sure something's going to happen soon..."),
            new DialogueLine("I should get straight to work!","talking_grinning")
        };

        script.Add(nameof(opening), opening);

        var openshop = new DialogueLine[]
        {
            new DialogueLine("If I remember correctly, there's a storage in the back..."),
            new DialogueLine("I can press space to interact."),
        };

        script.Add(nameof(openshop), openshop);

        var openinventory = new DialogueLine[]
        {
            new DialogueLine("Grandpa must have been preparing for the spring. Customers will have a seasonal preference for Sakura Mochi.","talking_reading"),
            new DialogueLine("Let's get those in stock.")
        };

        script.Add(nameof(openinventory), openinventory);

        var placeitem = new DialogueLine[] {
            new DialogueLine("Great! I should keep restocking throughout the day so everyone can get what they need."), 
            new DialogueLine("Once someone is done shopping, I need to go to the register to check them out.") 
        };

        script.Add(nameof(placeitem), placeitem);

        var forecast = new DialogueLine[]
        {
            new DialogueLine("The forecast is warning us of a wildfire soon!"),
            new DialogueLine("I'd better prepare...")
        };

        script.Add(nameof(forecast), forecast);

        var endofday = new DialogueLine[]
        {
            new DialogueLine("I should restock items with the money I have."),
            new DialogueLine("Customers will have a disaster preference for fire extinguishers, so I'll buy those for tomorrow.","talking_reading"),
            new DialogueLine("I can also purchase upgrades to protect the store from disasters."),
            new DialogueLine("After that's done, I can click the sleep button to move on to the next day.","talking_grinning")
        };

        script.Add(nameof(endofday), endofday);

        var disasterstats = new DialogueLine[]
        {
            new DialogueLine("Looks like the wildfire spread overnight..."),
            new DialogueLine("I lost quite a bit in the incident. I should be careful from now on.","talking_reading")
        };

        script.Add(nameof(disasterstats), disasterstats);
    }

    private void OnTutorialTimerTimeout()
    {
        StartTutorial("openshop");
    } 
        
    private async void OnInteractPressed()
    {
        if (isTyping)
        {
            isTyping = false;
            tutorialText.Text = script[currentTutorialName][currentStepIndex].Text;
        }
        else
        {
            currentStepIndex++;
            await ShowTutorialStep();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if(isTutorialPlaying)
        {
            if(@event.IsActionPressed("Interact"))
            {
                OnInteractPressed();
            }
        }
    }

    private async Task ShowTutorialStep()
    {
        Debug.Assert(script.ContainsKey(currentTutorialName), $"There is no tutorial with the name: {currentTutorialName}");
        if (currentStepIndex < script[currentTutorialName].Length)
        {
            var dialogue = script[currentTutorialName][currentStepIndex];
            animationPlayer.Play("RESET");
            animationPlayer.Advance(0);
            animationPlayer.Play(dialogue.AnimationName);
            string line = dialogue.Text;
            await TypeText(line);
        }
        else
        {
            GetTree().Paused = false;
            EndTutorial();
        }
    }

   

    private void OnSceneChanged(string sceneName)
    {
        switch (sceneName)
        {
            case "gamescene":
                OnGameSceneLoaded();
                break;
            case "seasontitle":
                OnSeasonSceneLoaded();
                break;
            case "forecastscene":
                OnForecastSceneLoaded();
                break;
            case "endofdayscene":
                OnEndOfDaySceneLoaded();
                break;
            case "disasterstatsscene":
                OnDisasterStatsSceneLoaded();
                break;
        }
    }

    private async void OnSeasonSceneLoaded()
    {
        await ToSignal(GetTree().CreateTimer(3), "timeout");
        StartTutorial("opening");
    }
    private void OnGameSceneLoaded()
    {
        if(isFirstDay)
        {
            SubscribeToCounterSignal();
            tutorialTimer.Start();
            storage = (storage)GetTree().GetFirstNodeInGroup("storage");
            storage.InventoryOpened += OnInventoryOpened;
            isFirstDay = false;
        }
        
    }
    private void SubscribeToCounterSignal()
    {
       var counters = GetTree().GetNodesInGroup("counters");

       foreach (counter counter in counters)
        {
            counter.TakeItemres += OnTakeItemres;
        }
    }

    private void OnTakeItemres()
    {
        if (!isFirstCounterStocked)
        {
            isFirstCounterStocked = true;
            StartTutorial("placeitem");
        }
    }

    private void OnInventoryOpened()
    {
        if (!isFirstTimeInventory)
        {
            isFirstTimeInventory = true;
            StartTutorial("openinventory");
        }
    }

    private void OnForecastSceneLoaded()
    {
        forecast forecast = (forecast)sceneManager.GetNode("SceneParent/Forecast");
        forecast.ForecastEnded += OnForecastEnded;
    }

    private async void OnEndOfDaySceneLoaded()
    {
        if (!isFirstTimeEndOfDay)
        {
            isFirstTimeEndOfDay = true;
            await ToSignal(GetTree().CreateTimer(1), "timeout");
            StartTutorial("endofday");
        }
    }

    private void OnDisasterStatsSceneLoaded()
    {
        //await ToSignal(GetTree().CreateTimer(1), "timeout");
        StartTutorial("disasterstats");
    }

    private void OnForecastEnded()
    {
        StartTutorial("forecast");
    }

    private void EndTutorial()
    {
        EmitSignal(nameof(TutorialFinished));

        GetTree().Paused = false;
        isTutorialPlaying = false;
        Hide();

        GetViewport().SetInputAsHandled();
    }

    public void StartTutorial(string tutorialName)
    {
        GetTree().Paused = true;
        isTutorialPlaying = true;

        currentStepIndex = 0;
        GD.Print("Starting tutorial. Name: " + tutorialName);
        currentTutorialName = tutorialName;
        Show();
        ShowTutorialStep();
    }

    private async Task TypeText(string text)
    {
        tutorialText.Text = "";
        isTyping = true;
        foreach (char c in text)
        {
            if (!isTyping)
                break;

            tutorialText.Text += c;
            await ToSignal(GetTree().CreateTimer(textDelay), "timeout");
            audioStreamPlayer.Play();
        }

        isTyping = false;
        animationPlayer.Stop();
    }
} 