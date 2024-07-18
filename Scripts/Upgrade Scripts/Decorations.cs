using Godot;
using System;

public partial class Decorations : Upgrade
{
	[Export] public int extendedWaitTime;
	public override void onLevelLoad(globals Global, Node root) {
		root.GetChild<npcSpawner>(3).npcWaitTime = extendedWaitTime;

		// SHOW DECORATIONS AS WELL
	}
}
