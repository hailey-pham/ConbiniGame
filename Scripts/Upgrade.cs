using Godot;
using System;

public partial class Upgrade : Resource
{
	[Export] public string name;
	[Export(PropertyHint.MultilineText)] public string description;
	[Export] public bool owned; // if the upgrade is owned: true
	[Export] public int cost;

	public Upgrade() : this("", "", false, 0) { }
	public Upgrade(String _name, String _description, bool _owned, int _cost) {
		name = _name;
		description = _description;
		owned = _owned;
		cost = _cost;
	}

	public string returnName() {
		return name;
	}
	public virtual void onGameStart(globals Global, Node root) { }

	public virtual void onLevelLoad(globals Global, Node root) { } // World.cs 

	public virtual void onExteriorLoad(globals Global, Node root) { } // Storefront.cs

	public virtual void onDayEnd(globals Global, Node root) { } // EndOfDay.cs

	public virtual void onDisaster(globals Global, Node root, Calendar.DisastersEnum disaster) { } // disaster.cs

	public virtual void onDisasterStats(globals Global, disaster_stats root, Calendar.DisastersEnum disaster) { } // disaster_stats.cs
}
