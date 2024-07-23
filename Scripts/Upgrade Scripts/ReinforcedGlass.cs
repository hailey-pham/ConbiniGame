using Godot;
using System;

public partial class ReinforcedGlass : Upgrade
{
	[Export] public int itemProtection;
	[Export] public int moneyProtection;
	public override void onDisasterStats(globals Global, disaster_stats root, Calendar.DisastersEnum disaster) {
		if(disaster == Calendar.DisastersEnum.Earthquake || disaster == Calendar.DisastersEnum.Typhoon) {
			Global.ItemProtection += itemProtection;
			root.DisasterProtection += moneyProtection;
		} 
	} 
}
