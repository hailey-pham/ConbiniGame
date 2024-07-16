using Godot;
using System;

public partial class UpgradeButton : Button
{
	[Export] String upgradeName;
	[Export] CanvasLayer purchasePopUp;
	[Export] UpgradeManager manager;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		purchasePopUp.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	// public override void _Process(double delta)
	// {
	// }

	public void onPress()
	{
		GD.Print("UPGRADE BUTTON PRESSED");
		// GD.Print(manager);
		// manager.upgradeName = upgradeName;
		// GD.Print("NAME: "+manager.upgradeName);
		// purchasePopUp.Visible = true;
	}
}
