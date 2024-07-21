using Godot;
using System;
using System.Collections.Generic;

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

    private int disasterProtection = 0;

    private Calendar calendar;

    Random rnd = new Random();
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
        calendar = GetNode<Calendar>("/root/Calendar");

        globals = GetNode<globals>("/root/Globals");

        disasterProtection = 0;

		foreach (KeyValuePair<string, Upgrade> upgrade in globals.Upgrades) {
            if (upgrade.Value.owned) {
                upgrade.Value.onDisasterStats(globals,this, calendar.GetCurrentDayDisaster());
            }
        }

        DisplayDisasterStats();

    }

    private void OnContinueButtonPressed()
    {
        GD.Print("Continue button pressed!");
        EmitSignal(nameof(DisasterScreenEnded));
        sceneManager.ChangeScene("gamescene", "FadeToBlack");
    }

    private void OnDisasterScreenEnded()
    {
        GD.Print("Transitioning to main game...");
        sceneManager.ChangeScene("gamescene", "FadeToBlack");
    }

    private void OnRestockButtonPressed()
    {
        GD.Print("Restock button pressed!");
        EmitSignal(nameof(RestockButtonPressed));
        sceneManager.ChangeScene("restock");
        // logic here later i dont want to mess up the game
    }

    public void DisplayDisasterStats()
    {
        var globals = GetNode<globals>("/root/Globals");

        LoseMoney();
        CurrentMoney();
        disasterLabel.Text = "Disaster has struck your store. You have lost a portion of your resources...";
        // messageLabel.Text = "You lost a portion of your resources.";
        statsLabel.Text = "Money lost:  ￥" + newMoney.ToString();
        currentStatsLabel.Text = "Current funds: ￥" + globals.Money.ToString();
        itemsLostLabel.Text = "You lost " + (globals.stockLosePercentage * 100) + "% of each item";
    }

    private int LoseMoney()
    {
        
        newMoney = globals.Money - (globals.Money / rnd.Next(10+disasterProtection, 100)); // lose 1-10% of your money, 
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


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

    public int DisasterProtection
	{
		get { return disasterProtection; }
		set { disasterProtection = value; }
	}
}
