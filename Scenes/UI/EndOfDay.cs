
using Godot;
using System;

public partial class EndOfDay : Control
{
    [Signal]
	public delegate void UpgradeButtonPressedEventHandler();

    [Signal]
    public delegate void SleepButtonPressedEventHandler();

	private Button upgradeButton;
	private Button sleepButton;

	private RichTextLabel customersLabel;
	private RichTextLabel earningsLabel;
	private RichTextLabel dayLabel;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        upgradeButton = GetNode<Button>("VBoxContainer/HBoxContainer/UpgradeButton");
        sleepButton = GetNode<Button>("VBoxContainer/HBoxContainer/SleepButton");

        upgradeButton.Pressed += OnUpgradeButtonPressed;
        sleepButton.Pressed += OnSleepButtonPressed;

        customersLabel = GetNode<RichTextLabel>("VBoxContainer/Customers");
        earningsLabel = GetNode<RichTextLabel>("VBoxContainer/Cash");
        dayLabel = GetNode<RichTextLabel>("VBoxContainer/Day");
    }

    private void OnUpgradeButtonPressed()
	{
		GD.Print("Upgrade button pressed!");
        // logic for getting upgrade menu up
        EmitSignal(nameof(UpgradeButtonPressedEventHandler));
    }

    private void OnSleepButtonPressed()
	{
		GD.Print("Sleep button pressed!");
        // logic for playing sleep scene
        EmitSignal(nameof(SleepButtonPressedEventHandler));
    }

    public void SetStats(string customers, string earnings, string day)
    {
        customersLabel.Text = customers;
        earningsLabel.Text = earnings;
        dayLabel.Text = day;
    }

}