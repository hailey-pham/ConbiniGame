using Godot;
using System;

public partial class Advertisement : Upgrade
{
	[Export] public float curveIncrease;

	public virtual void onExteriorLoad(globals Global, Node root) { } // NEEDS  VISUALS

	public override void onLevelLoad(globals Global, Node root) {
		Curve curve = root.GetChild<npcSpawner>(4).spawnCurve;
		curve.SetPointValue(0, curve.GetPointPosition(0).Y+curveIncrease);
		curve.SetPointValue(1, curve.GetPointPosition(1).Y+curveIncrease);


	}
}
