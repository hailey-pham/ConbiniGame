using Godot;
using System;

public abstract partial class UpgradeFunction : Node
{
	public abstract void onGameStart(globals Global);

	public abstract void onLevelLoad(globals Global);

	public abstract void onExteriorLoad(globals Global);

	public abstract void onDayStart(globals Global);

	public abstract void onDayEnd(globals Global);
	
	public abstract void onDisaster(globals Global);

	
}
