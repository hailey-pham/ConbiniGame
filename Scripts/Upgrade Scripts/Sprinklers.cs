using Godot;
using System;

public partial class Sprinklers : Upgrade
{
	[Export] public int protection;
	public override void onDisasterStats(globals Global, disaster_stats root, Calendar.DisastersEnum disaster) {
		if(disaster == Calendar.DisastersEnum.WildFire) {
			root.DisasterProtection += protection;
		} 
	} 
}
