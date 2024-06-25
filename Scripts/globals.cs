using Godot;
using System;
using System.Collections.Generic;
using System.IO;


public partial class globals : Node
{
	private static int _money;
	public static Dictionary<string, ItemRes> _stock = new Dictionary<string, ItemRes>(); 
	public static int _purchaseCost;

	//signal to tell money GUI to update
	[Signal]
	public delegate void MoneyUpdatedEventHandler(int money);

	public int Money
	{
		get { return _money; }
		set {
			_money = value;
			EmitSignal(nameof(MoneyUpdated), _money);
        }
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		string path = "res://Resources/";
		var dir = DirAccess.Open(path);

		string[] fileNames = dir.GetFiles();
		foreach (string fileName in fileNames) {
			ItemRes resource = GD.Load<ItemRes>(path+fileName);
			_stock.Add(resource.name, resource);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
