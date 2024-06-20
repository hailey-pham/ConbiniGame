using Godot;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;


public partial class RestockManager : Node
{	
	Label totalCostLabel;
	Label currentMoneyLabel;
	private bool purchaseMade;

	public override void _Ready()
	{
		GiveMoney();
		purchaseMade = false;
		totalCostLabel = GetNode<Label>("TotalCost");
		currentMoneyLabel = GetNode<Label>("CurrentMoney");
		currentMoneyLabel.Text = string.Format("Current: ¥"+globals.Money);

	}

	private void GiveMoney() {
		globals.Money = 30;
		globals._stock["Water"].currentStock = 2;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		totalCostLabel.Text = string.Format("Total Cost: ¥"+globals._purchaseCost);

	}

	public void completePurchase() {
		if(!purchaseMade) {
			purchaseMade = true;
			// check if user has enough funds to restock
			// add restock amounts to global inventory
			if(globals.Money > globals._purchaseCost) {
				globals.Money -= globals._purchaseCost;
				globals._purchaseCost = 0;
				foreach (var item in globals._stock) {
					item.Value.currentStock += item.Value.restockAmount;
					item.Value.restockAmount = 0;
				}
			} else {
				GD.Print("not enough money for purchase");
			}	
		}
		
	}
}
