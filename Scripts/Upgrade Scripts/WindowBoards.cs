using Godot;
using System;

public partial class WindowBoards : Upgrade
{
	[Export] public int protection;
	public override void onExteriorLoad(globals Global, Node root) {
		TextureRect vending = (TextureRect)root.GetTree().GetFirstNodeInGroup("Boards");
		vending.Visible = true;
	} // Storefront.cs
	public override void onDisasterStats(globals Global, disaster_stats root, Calendar.DisastersEnum disaster) {
		if(disaster == Calendar.DisastersEnum.Typhoon) {
			root.DisasterProtection += protection;
			this.owned = false; // one time use
		} else if (disaster == Calendar.DisastersEnum.WildFire) { // gets destroyed in fires
			this.owned = false;
		}
	} 
}
