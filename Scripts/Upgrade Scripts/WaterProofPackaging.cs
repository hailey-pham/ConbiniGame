using Godot;
using System;

public partial class WaterProofPackaging : Upgrade
{
	[Export] public int protection;
	public override void onDisasterStats(globals Global, disaster_stats root, Calendar.DisastersEnum disaster) {
		if(disaster == Calendar.DisastersEnum.Typhoon || disaster == Calendar.DisastersEnum.Tsunami || disaster == Calendar.DisastersEnum.FlashFlood) {
			Global.ItemProtection += protection;
		} 
	} 
}
