using Godot;
using System;
using System.Collections.Generic;


public partial class CloseEarly : Button
{
	globals globals;
	Calendar calendar;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.Visible = false;
		globals = GetNode<globals>("/root/Globals");
		calendar = GetNode<Calendar>("/root/Calendar");
		this.Pressed += onPressed;
		globals.StockUpdated += onStockSignal;
	}

	public void onStockSignal()
    {
		GD.Print("StockModified");
		bool outOfStock = true;
		foreach(KeyValuePair<string, ItemRes> item in globals.Stock) {
			if(item.Value.currentStock != 0) {
				outOfStock = false;
				break;
			}
		}

		if(outOfStock && !this.Visible) {
			this.Visible = true;

			Tween tween = this.CreateTween();
			tween.TweenInterval(0.1);
			tween.SetEase(Tween.EaseType.Out);
			Modulate = new Color("ffffff00");
			tween.TweenProperty(this, "modulate", new Color("ffffffff"), 1f);
		}
    }

	public void onPressed() {
		calendar.EndDayEarly();
	}

}
