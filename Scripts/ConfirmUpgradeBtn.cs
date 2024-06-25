using Godot;
using System;

public partial class ConfirmUpgradeBtn : Button
{
	[Export] CanvasLayer PopUp;
	[Export] UpgradeManager manager;

	private globals globals;


	Upgrade currentUpgrade;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		globals = GetNode<globals>("/root/Globals");

	}

	public void onPress() {
		GD.Print(manager.upgradeName);
		currentUpgrade = globals.Upgrades[manager.upgradeName];
		GD.Print("Current Upgrade Set");
		GD.Print(currentUpgrade.name);
		if(globals.Money > currentUpgrade.cost) {
			GD.Print("updating...");
			globals.Money -= currentUpgrade.cost;
			globals.Upgrades[currentUpgrade.name].owned = true;
		}
		
		GD.Print("Set Visibility");
		PopUp.Visible = false;
	}

	
}
