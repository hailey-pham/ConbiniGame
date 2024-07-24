
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

    private float textDelay = 0.05f;
    private bool isTyping = false;
    private int part = 0;

    private bool isTutorialPlaying = false;
    private bool isFirstCounterStocked = false;
    private bool isFirstTimeInventory = false;

    private SceneManager sceneManager;
    private counter counter;
    private storage storage;

    [Signal]
    public delegate void TutorialFinishedEventHandler();

    private Dictionary<string, string[]> script = new();
      

    public override void _Ready()
    {
        //this must be called before the script is read from.
        BuildScript();

        Hide();
        sceneManager = GetNode<SceneManager>("/root/SceneManager");
        sceneManager.SceneChanged += OnSceneChanged;
        tutorialText = GetNode<RichTextLabel>("TutorialText");

        tutorialTimer = new Timer();
        tutorialTimer.WaitTime = 3.0f;
        tutorialTimer.OneShot = true;
        tutorialTimer.Timeout += OnTutorialTimerTimeout;
        AddChild(tutorialTimer);

        ProcessMode = ProcessModeEnum.Always;
    }

    //adds all the text to our script dictionary
    private void BuildScript()
    {
        var opening = new string[]
        {
            "Grandpa passed away in the winter.",
            "He owned this store...",
            "Apparently I'm the one to inherit it.",
            "He lived in this village, prone to disasters. I'm sure something's going to happen soon...",
            "I should get straight to work!"
        };

        script.Add(nameof(opening), opening);

        var openshop = new string[]
        {
            "If I remember correctly, there's a storage in the back...",
            "I can press space to interact.",
        };

        script.Add(nameof(openshop), openshop);

        var openinventory = new string[]
        {
            "Grandpa must have been preparing for the spring. Customers will have a seasonal preference for sakura mochi.",
            "Let's get those in stock."
        };

        script.Add(nameof(openinventory), openinventory);

        var placeitem = new string[]
        {
            "Great! I should keep restocking throughout the day so everyone can get what they need.",
            "Once someone is done shopping, I need to go to the register to check them out."
        };

        script.Add(nameof(placeitem), placeitem);
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
            tutorialText.Text = script[currentTutorialName][currentStepIndex];
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
            string line = script[currentTutorialName][currentStepIndex];
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
        }
    }

    private async void OnSeasonSceneLoaded()
    {
        await ToSignal(GetTree().CreateTimer(3), "timeout");
        StartTutorial("opening");
    }
    private void OnGameSceneLoaded()
    {
        SubscribeToCounterSignal();
        tutorialTimer.Start();
        storage = (storage)GetTree().GetFirstNodeInGroup("storage");
        storage.InventoryOpened += OnInventoryOpened;
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
            GetTree().Paused = true;
            isFirstCounterStocked = true;
            StartTutorial("placeitem");
        }
    }

    private void OnInventoryOpened()
    {
        if (!isFirstTimeInventory)
        {
            GetTree().Paused = true;
            isFirstTimeInventory = true;
            StartTutorial("openinventory");
        }
    }

    private void EndTutorial()
    {
        EmitSignal(nameof(TutorialFinished));

        GetTree().Paused = false;
        isTutorialPlaying = false;
        Hide();
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
        }

        isTyping = false;
    }
} 