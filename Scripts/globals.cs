using Godot;
using System;
using System.Collections.Generic;
using System.IO;


public partial class globals : Node
{
	private static int _money;
    private static Dictionary<string, ItemRes> stock = new Dictionary<string, ItemRes>();
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

    public static Dictionary<string, ItemRes> Stock { get => stock; set => stock = value; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		string path = "res://Resources/Items/";
		var dir = DirAccess.Open(path);

		string[] fileNames = dir.GetFiles();
		GD.Print("Getting resources...");

		string tempFileName;
		char[] remap = {'.','r','e','m','a','p'};
		foreach (string fileName in fileNames) {
			GD.Print("Adding resource (item)...");
			if (fileName.Contains(".tres.remap")) { 
    			tempFileName = fileName.TrimEnd(remap);
				GD.Print("TFN: "+tempFileName);
			} else {
				tempFileName = fileName;
			}
			ItemRes resource = GD.Load<ItemRes>(path+tempFileName);
			GD.Print(resource.name);
			Stock.Add(resource.name, resource);
		}

		string pathU = "res://Resources/Upgrades/";
		var dirU = DirAccess.Open(pathU);

		string[] fileNamesU = dirU.GetFiles();
		GD.Print(pathU);
		GD.Print("Getting resources...");

		foreach (string fileName in fileNamesU) {
			if (fileName.Contains(".tres.remap")) { 
    			tempFileName = fileName.TrimEnd(remap);
				
			} else {
				tempFileName = fileName;
			}

			GD.Print("Adding resource (item)...");
			Upgrade resourceU = GD.Load<Upgrade>(pathU+tempFileName);
			GD.Print(resourceU.name);
			_upgrades.Add(resourceU.name, resourceU);
		}

		//sets water stock to five if we're in debug mode
#if DEBUG
		foreach (var item in stock.Values)
		{
			item.currentStock = 999;
		}
#endif
	}

    public void ResetEarnings()
    {
        Earnings = 0;
    }
	
	public void ResetCustomers()
	{
		Customers = 0;
	}
    public static void IncrementDay()
    {
        _day++;
    }

	public static void DecrementItemResStock(ItemRes itemRes)
	{
		stock[itemRes.name].currentStock = stock[itemRes.name].currentStock - 1;
    }

	public static void IncrementItemResStock(ItemRes itemRes)
	{
        stock[itemRes.name].currentStock = stock[itemRes.name].currentStock + 1;
    }
}
