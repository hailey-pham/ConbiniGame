using Godot;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;


public partial class RestockManager : Node
{	
	 [Signal]
    public delegate void BackButtonPressedEventHandler();

	private Button backButton;
	

	[Export] Label totalCostLabel;
	[Export] Label currentMoneyLabel;
	private bool purchaseMade;
	private globals globals;
	public override void _Ready()
	{
		globals = GetNode<globals>("/root/Globals");
		backButton = GetNode<Button>("BackButton");
		backButton.Pressed += OnBackButtonPressed;


        // GiveMoney();
		purchaseMade = false;
		// totalCostLabel = GetNode<Label>("TotalCost");
		// currentMoneyLabel = GetNode<Label>("CurrentMoney");
		currentMoneyLabel.Text = string.Format("Current: "+globals.Money+ " Y");
	}

	private void GiveMoney() {
		globals.Money = 30;
		globals.Stock["Water"].currentStock = 2;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		totalCostLabel.Text = string.Format("Total Cost: "+globals._purchaseCost+" Y");
	}

	public void completePurchase() {
		if(!purchaseMade) {
			purchaseMade = true;
			// check if user has enough funds to restock
			// add restock amounts to global inventory
			if(globals.Money > globals._purchaseCost) {
				globals.Money -= globals._purchaseCost;
				globals._purchaseCost = 0;
				foreach (var item in globals.Stock) {
					item.Value.currentStock += item.Value.restockAmount;
					item.Value.restockAmount = 0;
				}
				currentMoneyLabel.Text = string.Format("Current: "+globals.Money+" Y");
				purchaseMade = false;
				
			} else {
				GD.Print("not enough money for purchase");
				purchaseMade = false;
			}	
		}
		
	}

	
	private void OnBackButtonPressed()
    {
        // GD.Print("Back button pressed!");
        // todo later: logic for getting upgrade menu up
        EmitSignal(nameof(BackButtonPressed));
        var sceneManager = GetNode<SceneManager>("/root/SceneManager");
        // GD.Print(sceneManager);
		sceneManager.ChangeScene("endofdayscene");
    }
}
