using Godot;
using System;

public partial class Advertisement : Upgrade
{
	[Export] public float curveIncrease;

	public override void onExteriorLoad(globals Global, Node root) {
		TextureRect flag = (TextureRect)root.GetTree().GetFirstNodeInGroup("Flag");
		flag.Visible = true;
	} // Storefront.cs
	public override void onLevelLoad(globals Global, Node root) {
		Curve curve = ((npcSpawner)root.GetTree().GetFirstNodeInGroup("spawner")).spawnCurve;
		curve.SetPointValue(0, curve.GetPointPosition(0).Y+curveIncrease);
		curve.SetPointValue(1, curve.GetPointPosition(1).Y+curveIncrease);
	}
}
