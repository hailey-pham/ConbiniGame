using Godot;
using System;

public partial class StockButton : Button
{
	[Export] public string itemName;
	[Export] public bool addButton; // if true, add 1 to stock, if false, subtract 1
	// Called when the node enters the scene tree for the first time.
	
	public void onPress() {
		
		if(addButton) {
			// add
			globals._stock[itemName].restockAmount += 1;
			globals._purchaseCost += globals._stock[itemName].restockPrice;

		} else {
			// subtract
			if(globals._stock[itemName].restockAmount >= 1) {
				globals._stock[itemName].restockAmount -= 1;
				globals._purchaseCost -= globals._stock[itemName].restockPrice;
			}
			
		}
	}
}
