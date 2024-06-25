using Godot;
using System;

public partial class DenyUpgradeBtn : Button
{
	[Export] CanvasLayer PopUp;

	public void onPress() {
		PopUp.Visible = false;
		
	}
}
