using Godot;
using System;

public partial class RestockLabelManager : Label
{
	[Export] public string itemName;
	Label currentStockLabel;
	Label purchaseAmountLabel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		itemName = this.Text;

		purchaseAmountLabel = GetNode<Label>("PurchaseAmount");
		currentStockLabel = GetNode<Label>("CurrentStock");
		
		currentStockLabel.Text = string.Format("Current Stock: "+globals._stock[itemName].currentStock);
		purchaseAmountLabel.Text = string.Format(globals._stock[itemName].restockAmount.ToString());

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		purchaseAmountLabel.Text = string.Format(globals._stock[itemName].restockAmount.ToString());
	}
}
