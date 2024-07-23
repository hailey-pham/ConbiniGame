using Godot;
using System;

public partial class BetterHeating : Upgrade
{
	[Export] public int protection;
	public override void onDisasterStats(globals Global, disaster_stats root, Calendar.DisastersEnum disaster) {
		if(disaster == Calendar.DisastersEnum.HeavySnow) {
			Global.ItemProtection += protection;
		} 
	} 
}
