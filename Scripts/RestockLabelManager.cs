using Godot;
using System;

public partial class RestockLabelManager : Label
{
	[Export] public string itemName;
	Label currentStockLabel;
	Label purchaseAmountLabel;
	// Called when the node enters the scene tree for the first time.
	private globals globals;
	public override void _Ready()
	{
		globals = GetNode<globals>("/root/Globals");

		itemName = this.Text;
		
		purchaseAmountLabel = GetChild<Label>(2);
		currentStockLabel = GetChild<Label>(3);
		
		currentStockLabel.Text = string.Format("Current Stock: "+globals._stock[itemName].currentStock);
		purchaseAmountLabel.Text = string.Format(globals._stock[itemName].restockAmount.ToString());
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		purchaseAmountLabel.Text = string.Format(globals._stock[itemName].restockAmount.ToString());
		currentStockLabel.Text = string.Format("Current Stock: "+globals._stock[itemName].currentStock);

		
	}
}
