using Godot;
using System;

public partial class Upgrade : Resource
{
	[Export] public string name;
	[Export] public string description;
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
	public void onGameStart(globals Global) { }

	public void onLevelLoad(globals Global) { } // World.cs 

	public void onExteriorLoad(globals Global) { } // Storefront.cs

	public void onDayEnd(globals Global) { } // EndOfDay.cs

	public void onDisaster(globals Global, Calendar.DisastersEnum disaster) { } // disaster.cs
}
