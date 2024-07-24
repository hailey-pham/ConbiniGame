using Godot;
using System;

public partial class Decorations : Upgrade
{
	[Export] public int extendedWaitTime;
	public override void onLevelLoad(globals Global, Node root) {
		root.GetNode<npcSpawner>("npcSpawner").npcWaitTime = extendedWaitTime;
		TileMap tileMap = root.GetNode<TileMap>("/root/SceneManager/SceneParent/World/NavigationRegion2D/NewStore");
		//enables the decoration layer in the tilemap
		tileMap.SetLayerEnabled(1,true);
	}
}
