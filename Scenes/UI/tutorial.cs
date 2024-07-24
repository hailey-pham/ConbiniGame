
using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

public partial class tutorial : Control
{
    private RichTextLabel tutorialText;
    private string[] tutorialDialogue;
    private int currentStepIndex = 0;

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
        TutorialFinished += OnTutorialFinished;

        SubscribeToCounterSignal();

    }

    private void OnTutorialTimerTimeout()
    {
        ShowTutorial();
    } 
        
    private async void OnInteractPressed()
    {
        if (isTyping)
        {
            isTyping = false;
            tutorialText.Text = tutorialDialogue[currentStepIndex];
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
        if (currentStepIndex < tutorialDialogue.Length)
        {
            string line = tutorialDialogue[currentStepIndex];
            await TypeText(line);
        }
        else
        {
            EmitSignal(nameof(TutorialFinished));
        }
    }

   

    private void OnSceneChanged(string sceneName)
    {
        if (sceneName == "gamescene")
        {
            tutorialTimer.Start();
            storage = (storage)GetTree().GetFirstNodeInGroup("storage");
            storage.InventoryOpened += OnInventoryOpened;
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
            GetTree().Paused = true;
            isFirstCounterStocked = true;
            ShowTutorial();
        }
    }

    private void OnInventoryOpened()
    {
        if (!isFirstTimeInventory)
        {
            GetTree().Paused = true;
            isFirstTimeInventory = true;
            ShowTutorial();
        }
    }

    private void OnTutorialFinished()
    {
        part++;
        GetTree().Paused = false;
        isTutorialPlaying = false;
        Hide();
    }

    public void StartTutorial(int part)
    {
        GD.Print("Starting tutorial. Part: " + part);
        this.part = part;
          switch (part)
        {
            case 1:
                tutorialDialogue = new string[]
                {
                    "If I remember correctly, there's a storage in the back...",
                    "I can press space to interact.",
                };
                break;
            case 2:
                tutorialDialogue = new string[]
                {
                    "Grandpa must have been preparing for the spring. Customers will have a seasonal preference for sakura mochi.",
                    "Let's get those in stock."
                };
                break;
            case 3:
                tutorialDialogue = new string[]
                {
                    "Great! I should keep restocking throughout the day so everyone can get what they need.",
                    "Once someone is done shopping, I need to go to the register to check them out."
                };
                break;
        }
        currentStepIndex = 0;
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

    private void ShowTutorial()
    {
        isTutorialPlaying = true;
        GetTree().Paused = true;

        var tutorialScene = GD.Load<PackedScene>("res://Scenes/UI/tutorial.tscn");
        var tutorialInstance = tutorialScene.Instantiate<tutorial>();

        tutorialInstance.TutorialFinished += OnTutorialFinished;

        GetTree().Root.CallDeferred("add_child", tutorialInstance);

        StartTutorial(part);
    }

   
} 