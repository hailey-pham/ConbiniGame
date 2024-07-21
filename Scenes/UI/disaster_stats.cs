using Godot;
using System;

public partial class disaster_stats : Control
{
    [Signal]
    public delegate void DisasterScreenEndedEventHandler();
    [Signal]
    public delegate void RestockButtonPressedEventHandler();


    private RichTextLabel disasterLabel;
    private RichTextLabel messageLabel;
    private RichTextLabel statsLabel;
    private RichTextLabel currentStatsLabel;
    private RichTextLabel itemsLostLabel;

    private Button continueButton;
    private Button restockButton;

    private globals globals;
    private SceneManager sceneManager;

    private int newMoney;
    private int currentMoney;
    private int totalMoney;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        continueButton = GetNode<Button>("ContinueButton");
        continueButton.Pressed += OnContinueButtonPressed;
        restockButton = GetNode<Button>("RestockButton");
        restockButton.Pressed += OnRestockButtonPressed;

        disasterLabel = GetNode<RichTextLabel>("VBoxContainer/UhOh");
        messageLabel = GetNode<RichTextLabel>("VBoxContainer/Message");
        statsLabel = GetNode<RichTextLabel>("VBoxContainer/Stats");
        currentStatsLabel = GetNode<RichTextLabel>("VBoxContainer/CurrentStats");
        itemsLostLabel = GetNode<RichTextLabel>("VBoxContainer/ItemsLost");

        globals = GetNode<globals>("/root/Globals");
        sceneManager = GetNode<SceneManager>("/root/SceneManager");

        DisplayDisasterStats();

    }

    private void OnContinueButtonPressed()
    {
        GD.Print("Continue button pressed!");
        EmitSignal(nameof(DisasterScreenEnded));
        sceneManager.ChangeScene("gamescene");
    }

    private void OnDisasterScreenEnded()
    {
        GD.Print("Transitioning to main game...");
        sceneManager.ChangeScene("gamescene");
    }

    private void OnRestockButtonPressed()
    {
        GD.Print("Restock button pressed!");
        EmitSignal(nameof(RestockButtonPressed));

        // logic here later i dont want to mess up the game
    }

    public void DisplayDisasterStats()
    {
        var globals = GetNode<globals>("/root/Globals");

        LoseMoney();
        CurrentMoney();
        disasterLabel.Text = "Oh no! Disaster occured!";
        messageLabel.Text = "You lost a portion of your resources.";
        statsLabel.Text = "Money lost:  " + newMoney.ToString() + " yen";
        currentStatsLabel.Text = "You now have: " + globals.Money.ToString() + " yen";
        itemsLostLabel.Text = "You lost: " + (globals.stockLosePercentage * 100) + "% of each item";
    }

    private int LoseMoney()
    {
        newMoney = globals.Money / 3;
        return newMoney;
    }

    private int CurrentMoney()
    {
        globals.Money = globals.Money - newMoney;
        return globals.Money;
    }

    public void UpdateMoney()
    {
        totalMoney = CurrentMoney();
        globals.Money = totalMoney;
    }
}
