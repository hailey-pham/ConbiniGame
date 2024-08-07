using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Enumeration;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


public partial class globals : Node
{
    //emited by load game when all resources and items have been loaded
    [Signal]
    public delegate void GameLoadedEventHandler();

    private static int _money;
    private Dictionary<string, ItemRes> stock = new Dictionary<string, ItemRes>();
    public static int _purchaseCost;

    public int _holdMoneyLost;
    public bool sleepSkipped = false;

	//daily properties
	public static int _customersServed;
	public static int _earnings;
	private static List<ItemRes> _itemsSoldToday = new();
	public static int _day = 0;

    private int itemProtection = 0;
    

	private int _customersEntered = 0;

	public static double stockLosePercentage = 0.1; // make randomizing function for this l8r

	public static Dictionary<string, Upgrade> _upgrades = new Dictionary<string, Upgrade>();

	//signal to tell money GUI to update
	[Signal]
	public delegate void MoneyUpdatedEventHandler(int money);

    //signal for stock
    [Signal]
    public delegate void StockUpdatedEventHandler();

	private disaster_stats disasterstats;

    private bool lostMoneyToday = false;

    public int Money
	{
		get { return _money; }
		set {
			_money = value;
			EmitSignal(nameof(MoneyUpdated), _money);
        }
	}

	public int CustomersServed
	{
		get { return _customersServed; }
		set { _customersServed = value; }
	}

	public int CustomersEntered
	{
		get { return _customersEntered; }
		set { _customersEntered = value; }
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

    public Dictionary<string, ItemRes> Stock { 
        get { return stock; }
        set {
            stock = value;
            EmitSignal(nameof(StockUpdated));
        }
    }
    
    public static List<ItemRes> ItemsSoldToday { get => _itemsSoldToday; set => _itemsSoldToday = value; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		itemProtection = 0;


		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();

		LoadGameData();

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
		CustomersServed = 0;
	}
    public static void IncrementDay()
    {
        _day++;
    }

	public void ResetDayStats()
	{
        npcSpawner.npcCount = 0;
		ResetCustomers();
		ResetEarnings();
		ItemsSoldToday.Clear();
        MoneyLostToday = false;
	}

	public void DecrementItemResStock(ItemRes itemRes)
	{
		stock[itemRes.name].currentStock = stock[itemRes.name].currentStock - 1;
        EmitSignal(nameof(StockUpdated));
    }

	public void IncrementItemResStock(ItemRes itemRes)
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
			int minLoss = 1+Day*2;

        	int percentLoss = item.currentStock / rnd.Next(2+itemProtection, 10);

			int lossAmount;
			if (minLoss > percentLoss) {
				lossAmount = minLoss;
			} else {
				lossAmount = percentLoss;
			}

			item.LossAmount = Math.Min(item.currentStock, lossAmount);
            // cant let stock go negative
            item.currentStock = Math.Max(0, item.currentStock - lossAmount);
            GD.Print(item.name+": Items Lost ("+lossAmount+") Current Stock ("+item.currentStock+")");
		}
    }
    public int ItemProtection
    {
        get { return itemProtection; }
        set { itemProtection = value; }
    }

    public bool MoneyLostToday { get => lostMoneyToday; set => lostMoneyToday = value; }

    public void ResetGame()
	{
		GD.Print("SetGame!");
        foreach (var item in stock.Values)
        {
            item.currentStock = 0;
        }
        Money = 2000;
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
        Money = 999999;
        foreach (var item in stock.Values)
        {
            //temporarily commenting out so we don't have max stock on presentation
            //item.currentStock = 999;
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

    private async void LoadGameData()
    {
        var t = Task.Run(() =>
        {
            stock.Clear();

            string path = "res://Resources/Items/";
            var dir = DirAccess.Open(path);

            string[] fileNames = dir.GetFiles();
            GD.Print("Getting items...");

            foreach(var filename in fileNames)
            {
                string tempFileName;
                char[] remap = { '.', 'r', 'e', 'm', 'a', 'p' };

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
            }
        });

        var u = Task.Run(() =>
        {
            Upgrades.Clear();

            string pathU = "res://Resources/Upgrades/";
            var dirU = DirAccess.Open(pathU);

            string[] fileNamesU = dirU.GetFiles();
            GD.Print("Getting upgrades...");

            foreach(var fileName in fileNamesU)
            {
                string tempFileName;
                char[] remap = { '.', 'r', 'e', 'm', 'a', 'p' };

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

                if (resourceU == null)
                {
                    throw new Exception($"Resource at {pathU + tempFileName} failed to load!");
                }

                _upgrades.Add(resourceU.name, resourceU);
                GD.Print("Added resource (upgrade): " + resourceU.name);
            }
        });

        await t;
        await u;

        EmitSignal(nameof(GameLoaded));
    }
}
