using Godot;
using System;
using System.Collections.Generic;
public partial class StoreFront : Control
{
	// Called when the node enters the scene tree for the first time.
	globals globals;
	public override void _Ready()
	{
		globals = GetNode<globals>("/root/Globals");

		foreach (KeyValuePair<string, Upgrade> upgrade in globals.Upgrades) {
            if (upgrade.Value.owned) {
                upgrade.Value.onLevelLoad(globals, GetNode<Node>("."));
            }
        }
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
