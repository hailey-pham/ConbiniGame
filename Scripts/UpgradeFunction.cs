using Godot;
using System;

public abstract partial class UpgradeFunction : Node
{
	public abstract void onGameStart(globals Global);

	public abstract void onLevelLoad(globals Global); // World.cs 

	public abstract void onExteriorLoad(globals Global); // Storefront.cs

	public abstract void onDayEnd(globals Global); // EndOfDay.cs
	
	public abstract void onDisaster(globals Global, Calendar.DisastersEnum disaster); // disaster.cs

	
}
