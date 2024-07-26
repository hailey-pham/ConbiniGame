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
		int vendingCustomers = rnd.Next(5,(int)(Global.CustomersEntered/5.0));
		int vendingRevenue  = vendingCustomers*100;
		Global.Money += vendingRevenue;
		// Node vendingMachineNode = Global.FindChild("VendingMachineLabel", true);
		RichTextLabel vendingMachineLabel = (RichTextLabel)root.GetTree().GetFirstNodeInGroup("VendingLabel");
		vendingMachineLabel.Visible = true;
		vendingMachineLabel.Text = "(+￥"+vendingRevenue+" Vending Machine)";
	} // EndOfDay.cs
}
