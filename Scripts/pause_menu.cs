using Godot;
using System;

public partial class pause_menu : Control
{
	private Button mainButton;
	private Button optionsButton;
	private Button quitButton;

	private Button confirmQuitButton;

	private Control mainRoot;
	private Control optionsRoot;
	private Control quitRoot;

	[Export]private Control[] tabs;

	private RichTextLabel daysSurvived;
	private RichTextLabel season;
	private RichTextLabel currentFunds;

	private TextureRect warningIcon;

	private Calendar calendar;
	private globals globals;

	[Export] private string[] SeasonStrings;
	
	



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// get nodes
		calendar = GetNode<Calendar>("/root/Calendar");
		globals = GetNode<globals>("/root/Globals");

		mainButton = GetNode<Button>("Tabs/HBoxContainer/Main");
		optionsButton = GetNode<Button>("Tabs/HBoxContainer/Options");
		quitButton = GetNode<Button>("Tabs/HBoxContainer/Quit");

		confirmQuitButton = GetNode<Button>("Quit/FinalQuit");

		mainRoot = GetNode<Control>("GameStats");
		optionsRoot = GetNode<Control>("Options");
		quitRoot = GetNode<Control>("Quit");

		// game stats
		daysSurvived = GetNode<RichTextLabel>("GameStats/VBoxContainer/DaysSurvived");
		season = GetNode<RichTextLabel>("GameStats/VBoxContainer/CalendarDay");
		currentFunds = GetNode<RichTextLabel>("GameStats/VBoxContainer/CurrentFunds");
		warningIcon = GetNode<TextureRect>("GameStats/Warning");

		// button assignments
		mainButton.Pressed += onMainPressed;
		optionsButton.Pressed += onOptionsPressed;
		quitButton.Pressed += onQuitPressed;
		confirmQuitButton.Pressed += quitGame;

		mainRoot.Visible = true;
		optionsRoot.Visible = false;
		quitRoot.Visible = false;
		updateStats();
		Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//update game stats
		if(Visible) {
			updateStats();
		}
	}

	private void updateStats() {
		int seasonIdx = calendar.GetCurrentSeason() - 1;
		daysSurvived.Text = globals._day + " Days Survived";
		season.Text = SeasonStrings[seasonIdx]+" "+calendar.CurrentDay;
		currentFunds.Text = "Current Funds: ￥"+globals.Money;

		if(calendar.IsNextDisasterDay()) {
			Calendar.DisastersEnum nextDisaster = calendar.GetNextDayDisaster();
			var image = GD.Load<CompressedTexture2D>("res://Assets/Forecasts/" + Enum.GetName(typeof(Calendar.DisastersEnum), nextDisaster) + "Forecast.png");
			warningIcon.Texture = image;
		} else {
			warningIcon.Texture = GD.Load<Texture2D>("res://Assets/GUI/Blank.png");
		}
		
	}

	private void onMainPressed() {
		switchTabs(mainRoot);
	}

	private void onOptionsPressed() {
		switchTabs(optionsRoot);
	}

	private void onQuitPressed() {
		switchTabs(quitRoot);
	}

	private void switchTabs(Control currentTab) {
		foreach(Control ctrl in tabs) {
			if(ctrl != currentTab) {
				ctrl.Visible = false;
			} else {
				ctrl.Visible = true;
			}
		}
	}

	private void quitGame() {
		//quit the game
		GetTree().Quit();
	}

	public override void _Input(InputEvent @event)
    {
        if(@event.IsActionReleased("Exit"))
        {
			Visible = !Visible;
			GetTree().Paused = !GetTree().Paused;
		}
    }
}
