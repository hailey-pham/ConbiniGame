using Godot;
using System;

public partial class VendingMachine : Upgrade
{
	Random rnd = new Random();
	globals globals;
	public override void onExteriorLoad(globals Global, Node root) {
		TextureRect vending = (TextureRect)root.GetTree().GetFirstNodeInGroup("Vending");
		vending.Visible = true;
	} // Storefront.cs
	

	public override void onDayEnd(globals Global, Node root) { // There is no additional UI for this, help	
		int vendingRevenue  = rnd.Next(1000, 5000);
		Global.Money += vendingRevenue;
		Node vendingMachineNode = Global.FindChild("VendingMachineLabel", true);
		RichTextLabel vendingMachineLabel = root.GetChild<VBoxContainer>(3).GetChild<RichTextLabel>(1);
		vendingMachineLabel.Visible = true;
		vendingMachineLabel.Text = "(+￥"+vendingRevenue+" Vending Machine)";
	} // EndOfDay.cs
}
