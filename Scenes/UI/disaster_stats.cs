using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

public partial class disaster_stats : Control
{
    [Signal]
    public delegate void DisasterScreenEndedEventHandler();
    [Signal]
    public delegate void RestockButtonPressedEventHandler();

    [Export] private PackedScene LostItemContainer;


    private RichTextLabel disasterLabel;
    private RichTextLabel statsLabel;
    private RichTextLabel currentStatsLabel;
    // private RichTextLabel itemsLostLabel;

    private Button continueButton;
    private Button restockButton;

    private globals globals;
    private SceneManager sceneManager;

    private HBoxContainer hbox;

    private int newMoney;
    private int currentMoney;
    private int totalMoney;

    private int disasterProtection = 0;

    private int moneyLost = 0;

    private Calendar calendar;

    Random rnd = new Random();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        continueButton = GetNode<Button>("ContinueButton");
        continueButton.Pressed += OnContinueButtonPressed;
        restockButton = GetNode<Button>("RestockButton");
        restockButton.Pressed += OnRestockButtonPressed;

        hbox = GetNode<HBoxContainer>("ScrollContainer/HBoxContainer");
        disasterLabel = GetNode<RichTextLabel>("VBoxContainer/UhOh");
        statsLabel = GetNode<RichTextLabel>("VBoxContainer/Stats");
        currentStatsLabel = GetNode<RichTextLabel>("VBoxContainer/CurrentStats");
        // itemsLostLabel = GetNode<RichTextLabel>("VBoxContainer/ItemsLost");

        globals = GetNode<globals>("/root/Globals");
        sceneManager = GetNode<SceneManager>("/root/SceneManager");
        calendar = GetNode<Calendar>("/root/Calendar");

        globals = GetNode<globals>("/root/Globals");

        disasterProtection = 0;
        globals.ItemProtection = 0;

		foreach (KeyValuePair<string, Upgrade> upgrade in globals.Upgrades) {
            if (upgrade.Value.owned) {
                upgrade.Value.onDisasterStats(globals,this, calendar.GetCurrentDayDisaster());
            }
        }

        DisplayDisasterStats();

    }

    private void OnContinueButtonPressed()
    {
        //Theoretically if you start the day without items or stock you will lose the game :3
        bool emptyStock = true;
        if(globals.Money < 100)
        {
            foreach (var item in globals.Stock.Values)
            {
                if(item.currentStock != 0)
                {
                    emptyStock = false;
                }
            }
            if(emptyStock)
            {
                GD.Print("Game Over!");
                EmitSignal(nameof(DisasterScreenEnded));
                sceneManager.ChangeScene("gameover", "FadeToBlack");
                return;
            }
        }
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

        if(sceneManager.PrevScene != "disasterstatsscene") {
            GD.Print(sceneManager.PrevScene);
            LoseMoney();
            globals.LoseStock();
            CurrentMoney();
        }
        
        disasterLabel.Text = "Disaster has struck your store. You have lost a portion of your resources...";
        statsLabel.Text = "Money lost:  ￥" + moneyLost.ToString();
        currentStatsLabel.Text = "Current funds: ￥" + globals.Money.ToString();
        // itemsLostLabel.Text = "You lost " + (globals.stockLosePercentage * 100) + "% of each item";
        Node tempItem;
        foreach(KeyValuePair<string,ItemRes> item in globals.Stock) {
            if(item.Value.LossAmount > 0) {
                tempItem = LostItemContainer.Instantiate();
                tempItem.GetChild<TextureRect>(0).Texture = item.Value.spriteTexture;
                tempItem.GetChild<Label>(1).Text = "x"+item.Value.LossAmount.ToString();
                hbox.AddChild(tempItem);
            }
            
        }
    }

    private int LoseMoney()
    {
        //if we haven't already lost money today...
        //stored in globals so data persists between scene loading
        if(!globals.MoneyLostToday)
        {
            int minLoss = 100 + globals.Day * 200;
            int percentLoss = globals.Money / rnd.Next(2 + disasterProtection, 10); // lose 10-50% of your money, 

            if (minLoss > percentLoss)
            {
                moneyLost = minLoss;
            }
            else
            {
                moneyLost = percentLoss;
            }

            newMoney = globals.Money - moneyLost;
            globals.MoneyLostToday = true;
        }
        
        return newMoney;
    }

    private int CurrentMoney()
    {
        globals.Money = newMoney;
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
