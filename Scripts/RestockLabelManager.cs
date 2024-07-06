using Godot;
using System;

public partial class RestockLabelManager : Label
{
	[Export] public string itemName;
	[Export] Label currentStockLabel;
	[Export] Label purchaseAmountLabel;
	// Called when the node enters the scene tree for the first time.
	private globals globals;
	public override void _Ready()
	{
		globals = GetNode<globals>("/root/Globals");

		itemName = this.Text;

		purchaseAmountLabel = GetNode<Label>("PurchaseAmount");
		currentStockLabel = GetNode<Label>("CurrentStock");
		
		currentStockLabel.Text = string.Format("Current Stock: "+globals.Stock[itemName].currentStock);
		purchaseAmountLabel.Text = string.Format(globals.Stock[itemName].restockAmount.ToString());
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		purchaseAmountLabel.Text = string.Format(globals.Stock[itemName].restockAmount.ToString());
		currentStockLabel.Text = string.Format("Current Stock: "+globals.Stock[itemName].currentStock);

		
	}
}
