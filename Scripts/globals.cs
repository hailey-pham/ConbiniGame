using Godot;
using System;
using System.Collections.Generic;
using System.IO;


public partial class globals : Node
{
	private static int _money;
	public static Dictionary<string, ItemRes> _stock = new Dictionary<string, ItemRes>(); 
	
	
	//signal to tell money GUI to update
	[Signal]
	public delegate void MoneyUpdatedEventHandler(int money);
	public static int _purchaseCost;
	
	private static Dictionary<string, Upgrade> _upgrades = new Dictionary<string, Upgrade>(); 

	public static Dictionary<string, Upgrade> Upgrades
	{
		get { return _upgrades; }
		set { _upgrades = value; }
	}

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
		string path = "res://Resources/Items/";
		var dir = DirAccess.Open(path);

		string[] fileNames = dir.GetFiles();
		foreach (string fileName in fileNames) {
			ItemRes resource = GD.Load<ItemRes>(path+fileName);
			_stock.Add(resource.name, resource);
		}

		string pathU = "res://Resources/Upgrades/";
		var dirU = DirAccess.Open(pathU);

		string[] fileNamesU = dirU.GetFiles();
		GD.Print(pathU);
		foreach (string fileName in fileNamesU) {
			GD.Print("Getting resource...");
			Upgrade resourceU = GD.Load<Upgrade>(pathU+fileName);
			GD.Print(resourceU.name);
			_upgrades.Add(resourceU.name, resourceU);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
