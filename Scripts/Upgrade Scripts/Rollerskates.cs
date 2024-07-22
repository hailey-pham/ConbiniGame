using Godot;
using System;

public partial class Rollerskates : Upgrade
{
	[Export] float newSpeed;
	public override void onLevelLoad(globals Global, Node root) {
		player Player = (player)root.GetTree().GetFirstNodeInGroup("player");
        Player.setSpeed(newSpeed); 
		
	}
}
