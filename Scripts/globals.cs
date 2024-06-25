using Godot;
using System;
using System.Collections.Generic;
using System.IO;


public partial class globals : Node
{
	private static int _money;
	public static Dictionary<string, ItemRes> _stock = new Dictionary<string, ItemRes>(); 
	public static int _purchaseCost;

	public static int _customers;
	public static int _earnings;
	public static int _day = 0;

	public static Dictionary<string, Upgrade> _upgrades = new Dictionary<string, Upgrade>();

	//signal to tell money GUI to update
	[Signal]
	public delegate void MoneyUpdatedEventHandler(int money);

    [Signal]
    public delegate void EarningsUpdatedEventHandler(int earnings);

    public int Money
	{
		get { return _money; }
		set {
			_money = value;
			EmitSignal(nameof(MoneyUpdated), _money);
        }
	}

	public int Customers
	{
		get { return _customers; }
		set { _customers = value; }
	}
    public static int Day
    {
        get { return _day; }
        set { _day = value; }
    }

    public int Earnings
    {
        get { return _earnings; }
        set
        {
            _earnings = value;
            EmitSignal(nameof(EarningsUpdated), _earnings);
        }
    }

    public static Dictionary<string, Upgrade> Upgrades
	{
		get { return _upgrades; }
		set {
			_upgrades = value;
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//reads resource files from directory and loads them into global dictionary
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

    public void ResetEarnings()
    {
        Earnings = 0;
    }

    public static void IncrementDay()
    {
        _day++;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
