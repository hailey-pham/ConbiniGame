using Godot;
using System;

public partial class PlayerStackItem : Upgrade
{
    public override void onLevelLoad(globals Global, Node root) 
    {
        //Changes the process mode from disabled to inherit (default)
        player Player = (player)root.GetTree().GetFirstNodeInGroup("player");
        Player.IsStackItemUpgrade = true; //Should give Item Spawner 2.
    }
}
