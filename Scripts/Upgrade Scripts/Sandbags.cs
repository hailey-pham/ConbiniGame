using Godot;
using System;
using System.Collections.Generic;


public partial class Sandbags : Upgrade
{
	[Export] public int protection;
	public override void onExteriorLoad(globals Global, Node root) {
		TextureRect vending = (TextureRect)root.GetTree().GetFirstNodeInGroup("Sandbags");
		vending.Visible = true;
	} // Storefront.cs
	public override void onDisasterStats(globals Global, disaster_stats root, Calendar.DisastersEnum disaster) {
		if(disaster == Calendar.DisastersEnum.Typhoon || disaster == Calendar.DisastersEnum.Tsunami || disaster == Calendar.DisastersEnum.Earthquake) {
			root.DisasterProtection += protection;
			this.owned = false; // one time use
		} 
	} 
}
