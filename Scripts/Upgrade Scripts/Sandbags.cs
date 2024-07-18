using Godot;
using System;
using System.Collections.Generic;


public partial class Sandbags : Upgrade
{
	[Export] public int protection;
	public virtual void onExteriorLoad(globals Global, Node root) { } // NEEDS  VISUALS
	public override void onDisasterStats(globals Global, disaster_stats root, Calendar.DisastersEnum disaster) {
		if(disaster == Calendar.DisastersEnum.Typhoon || disaster == Calendar.DisastersEnum.Earthquake) {
			root.DisasterProtection += protection;
			this.owned = false; // one time use
		} 
	} 
}
