using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


public partial class globals : Node
{
	private static int _money;
    private static Dictionary<string, ItemRes> stock = new Dictionary<string, ItemRes>();
    public static int _purchaseCost;

	//daily properties
	public static int _customers;
	public static int _earnings;
	private static List<ItemRes> _itemsSoldToday = new();
	public static int _day = 0;

	private int itemProtection = 0;

	public static double stockLosePercentage = 0.1; // make randomizing function for this l8r

	public static Dictionary<string, Upgrade> _upgrades = new Dictionary<string, Upgrade>();

	//signal to tell money GUI to update
	[Signal]
	public delegate void MoneyUpdatedEventHandler(int money);

	private disaster_stats disasterstats;

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
    public static List<ItemRes> ItemsSoldToday { get => _itemsSoldToday; set => _itemsSoldToday = value; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		itemProtection = 0;

		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();

		var t = Task.Run(() =>
		{
            string path = "res://Resources/Items/";
            var dir = DirAccess.Open(path);

            string[] fileNames = dir.GetFiles();
            GD.Print("Getting items...");

            string tempFileName;
            char[] remap = { '.', 'r', 'e', 'm', 'a', 'p' };
			Parallel.ForEach(fileNames, filename =>
			{
                if (filename.Contains(".tres.remap"))
                {
                    tempFileName = filename.TrimEnd(remap);
                    // GD.Print("TFN: "+tempFileName);
                }
                else
                {
                    tempFileName = filename;
                }
                ItemRes resource = GD.Load<ItemRes>(path + tempFileName);
                // GD.Print(resource.name);
                if (resource == null)
                {
                    throw new Exception($"Resource at {path + tempFileName} failed to load!");
                }

                Stock.Add(resource.name, resource);
				GD.Print($"Added resource (item): {resource.name}");
            });
        });

        var u = Task.Run(() =>
        {
            string pathU = "res://Resources/Upgrades/";
            var dirU = DirAccess.Open(pathU);

            string[] fileNamesU = dirU.GetFiles();
            GD.Print("Getting upgrades...");

            string tempFileName;
            char[] remap = { '.', 'r', 'e', 'm', 'a', 'p' };
            Parallel.ForEach(fileNamesU, fileName =>
            {
                if (fileName.Contains(".tres.remap"))
                {
                    tempFileName = fileName.TrimEnd(remap);

                }
                else
                {
                    tempFileName = fileName;
                }

                // GD.Print("Adding resource (item)...");
                Upgrade resourceU = GD.Load<Upgrade>(pathU + tempFileName);
                _upgrades.Add(resourceU.name, resourceU);
                GD.Print("Added resource (upgrade): " + resourceU.name);
            });
        });

        stopwatch.Stop();
        GD.Print($"Resource loading took: {stopwatch.ElapsedMilliseconds} ms");

        SceneManager sceneManager = GetNode<SceneManager>("/root/SceneManager");
        sceneManager.SceneChanged += OnSceneChanged;
    }

    private void ResetEarnings()
    {
        Earnings = 0;
    }
	
	private void ResetCustomers()
	{
		Customers = 0;
	}
    public static void IncrementDay()
    {
        _day++;
    }

	public void ResetDayStats()
	{
		ResetCustomers();
		ResetEarnings();
		ItemsSoldToday.Clear();
	}

	public static void DecrementItemResStock(ItemRes itemRes)
	{
		stock[itemRes.name].currentStock = stock[itemRes.name].currentStock - 1;
    }

	public static void IncrementItemResStock(ItemRes itemRes)
	{
        stock[itemRes.name].currentStock = stock[itemRes.name].currentStock + 1;
    }

	public void SellItem(ItemRes item)
	{
        Earnings += item.price;
		Money += item.price;

		//if we haven't seen the item we sold today
		if(!ItemsSoldToday.Contains(item))
		{
			ItemsSoldToday.Add(item);
		}
    }

	public void LoseStock()
	{
        Random rnd = new Random();

        foreach (var item in stock.Values)
		{
            int lossAmount = (item.currentStock / rnd.Next(10+itemProtection, 100));
            // cant let stock go negative
            item.currentStock = Math.Max(0, item.currentStock - lossAmount);
		}
    }
    public int ItemProtection
    {
        get { return itemProtection; }
        set { itemProtection = value; }
    }

	public void ResetGame()
	{
		GD.Print("SetGame!");
        foreach (var item in stock.Values)
        {
            item.currentStock = 0;
        }
		
        Money = 500;
		for (int i = 0; i < 20;i++)
		{
			if (i < 5)
			{
				IncrementItemResStock(stock["Sakura Mochi"]);
            }
            IncrementItemResStock(stock["Water"]);
            IncrementItemResStock(stock["Toilet Paper"]);
        }
#if DEBUG
        foreach (var item in stock.Values)
        {
            item.currentStock = 999;
        }
#endif
		
    }

    private void OnSceneChanged(string sceneName)
    {
        if(sceneName == "mainmenu")
        {
            ResetGame();
        }
    }
}
