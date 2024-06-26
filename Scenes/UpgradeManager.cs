using Godot;
using System;

public partial class UpgradeManager : Control
{
	 [Signal]
    public delegate void BackButtonPressedEventHandler();

	private Button backButton;
	public string upgradeName;

	[Export] public Label currentMoneyLabel;

	private globals globals;
	public override void _Ready()
	{
		globals = GetNode<globals>("/root/Globals");
		backButton = GetNode<Button>("BackButton");
		backButton.Pressed += OnBackButtonPressed;
		currentMoneyLabel.Text = string.Format("Current: "+globals.Money+ " Y");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		currentMoneyLabel.Text = string.Format("Current: "+globals.Money+ " Y");
	}

	private void OnBackButtonPressed()
    {
        GD.Print("Back button pressed!");
        // todo later: logic for getting upgrade menu up
        EmitSignal(nameof(BackButtonPressed));
        var sceneManager = GetNode<SceneManager>("/root/SceneManager");
        GD.Print(sceneManager);
		sceneManager.ChangeScene("endofdayscene");
    }
}
