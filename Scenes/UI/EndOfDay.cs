
using Godot;
using System;

public partial class EndOfDay : Control
{
	[Signal]
	public delegate void UpgradeButtonPressedEventHandler();

	[Signal]
	public delegate void RestockButtonPressedEventHandler();

	[Signal]
	public delegate void SleepButtonPressedEventHandler();


	private Button upgradeButton;

	private Button restockButton;
	private Button sleepButton;

	private RichTextLabel customersLabel;
	private RichTextLabel earningsLabel;
	private RichTextLabel dayLabel;
	private RichTextLabel totalMoneyLabel;

	private Label itemsSoldLabel;
	private ItemList itemsSoldList;

	private globals globals;
	private Calendar calendar;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		upgradeButton = GetNode<Button>("VBoxContainer/HBoxContainer/UpgradeButton");

		restockButton = GetNode<Button>("VBoxContainer/HBoxContainer/RestockButton");

		sleepButton = GetNode<Button>("VBoxContainer/HBoxContainer/SleepButton");

		upgradeButton.Pressed += OnUpgradeButtonPressed;
		restockButton.Pressed += OnRestockButtonPressed;
		sleepButton.Pressed += OnSleepButtonPressed;


		customersLabel = GetNode<RichTextLabel>("VBoxContainer/Customers");
		earningsLabel = GetNode<RichTextLabel>("VBoxContainer/Cash");
		dayLabel = GetNode<RichTextLabel>("VBoxContainer/Day");
		totalMoneyLabel = GetNode<RichTextLabel>("VBoxContainer/TotalMoney");

		itemsSoldLabel = GetNode<Label>("VBoxContainer2/ItemsSoldLabel");
		itemsSoldList = GetNode<ItemList>("VBoxContainer2/ItemsSoldList");

		globals = GetNode<globals>("/root/Globals");
		calendar = GetNode<Calendar>("/root/Calendar");

		DisplayEndOfDayStats();
	}

	private void OnUpgradeButtonPressed()
	{
		GD.Print("Upgrade button pressed!");
		// todo later: logic for getting upgrade menu up
		EmitSignal(nameof(UpgradeButtonPressed));
		var sceneManager = GetNode<SceneManager>("/root/SceneManager");
		sceneManager.ChangeScene("upgrade");
	}

	private void OnRestockButtonPressed()
	{
		GD.Print("Restock button pressed!");
		EmitSignal(nameof(RestockButtonPressed));

		var sceneManager = GetNode<SceneManager>("/root/SceneManager");
		sceneManager.ChangeScene("restock");
	}

	private void OnSleepButtonPressed()
	{
		GD.Print("Sleep button pressed!");
		// todo later: logic for playing sleep scene
		EmitSignal(nameof(SleepButtonPressed));
		calendar.DetermineNextDay();
	}

	public void DisplayEndOfDayStats()
	{
		customersLabel.Text = "Customers Serviced: " + globals.Customers.ToString();
		earningsLabel.Text = "Cash Earned: " + globals.Earnings.ToString() + " yen";
		dayLabel.Text = "End of Day: " + globals.Day.ToString();
		totalMoneyLabel.Text = "You now have: " + globals.Money.ToString() + " yen";
		UpdateItemsSold();
	}

	private void UpdateItemsSold()
	{
		var itemsSoldToday = globals.ItemsSoldToday;

		foreach (var item in itemsSoldToday)
		{
			var itemTags = item.GetTagsAsEmoji();
			itemsSoldList.AddItem(item.name + " " + itemTags, item.spriteTexture, false);
		}
	}
}
