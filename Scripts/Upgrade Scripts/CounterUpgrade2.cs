using Godot;
using System;

public partial class CounterUpgrade2 : Upgrade
{
    public override void onLevelLoad(globals Global, Node root)
    {
        World World = (World)root.GetTree().GetFirstNodeInGroup("THEWORLD");
        World.counterState = 2;
        World.bothCounterUpgrades = true;
    }
}
