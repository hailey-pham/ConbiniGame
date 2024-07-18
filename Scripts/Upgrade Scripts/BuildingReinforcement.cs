using Godot;
using System;
public partial class BuildingReinforcement : Upgrade
{
	[Export] public int highProtection;
	[Export] public int lowProtection;
	public override void onDisasterStats(globals Global, disaster_stats root, Calendar.DisastersEnum disaster) {
		if(disaster == Calendar.DisastersEnum.Earthquake) {
			root.DisasterProtection += highProtection;
		} else if (disaster == Calendar.DisastersEnum.FlashFlood || disaster == Calendar.DisastersEnum.Typhoon || disaster == Calendar.DisastersEnum.Tsunami) {
			root.DisasterProtection += lowProtection;
		}
	} 
}
