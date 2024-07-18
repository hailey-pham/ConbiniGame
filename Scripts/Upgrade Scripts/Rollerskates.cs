using Godot;
using System;

public partial class Rollerskates : Upgrade
{
	[Export] float newSpeed;
	public override void onLevelLoad(globals Global, Node root) {
		root.GetChild<player>(2).setSpeed(newSpeed);
	}
}
