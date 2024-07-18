using Godot;
using System;

public partial class FloodProofDoors : Upgrade
{
	[Export] public int highProtection;
	[Export] public int lowProtection;
	public override void onDisasterStats(globals Global, disaster_stats root, Calendar.DisastersEnum disaster) {
		if(disaster == Calendar.DisastersEnum.FlashFlood || disaster == Calendar.DisastersEnum.Tsunami) {
			root.DisasterProtection += highProtection;
		} else if (disaster == Calendar.DisastersEnum.Typhoon) {
			root.DisasterProtection += lowProtection;
		}
	} 
}
