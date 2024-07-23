using Godot;
using System;
using System.Collections.Generic;

public partial class ItemRes : Resource
{
	[Export] public String name;
	[Export] public int price;
	[Export] public int restockPrice;

    private int lossAmount;

    //flags to designate seasonal items
    [Flags]
    public enum SeasonsEnum
    {
        Spring = 1 << 1,
        Summer = 1 << 2,
        Fall = 1 << 3,
        Winter = 1 << 4,
    }
    [Export]
    public SeasonsEnum SeasonFlags { get; set; }

    //flags to designate disaster items
    [Flags]
    public enum DisastersEnum
    {
        Earthquake = 1 << 1,
        Tsunami = 1 << 2,
        Typhoon = 1 << 3,
        WildFire = 1 << 4,
        FlashFlood = 1 << 5,
        HeavySnow = 1 << 6,
    }

    [Export]
    public DisastersEnum DisasterFlags { get; set; }

    [Export] public Texture2D spriteTexture;

    public int restockAmount = 0;
	public int currentStock = 0;

	public ItemRes() : this("",0,0,0,0) {}

	public ItemRes(String _name, int _price, int _restockPrice, int _restockAmount, int _currentStock) {
		name = _name;
		price = _price;
		restockPrice = _restockPrice;
		restockAmount = _restockAmount;
		currentStock = _currentStock;
	}

    public void ReturnItemToStock()
    {
        //add one back to the global stock
        globals.Stock[name].currentStock++;
    }

    public int LossAmount
	{
		get { return lossAmount; }
		set { lossAmount = value; }
	}

    public string GetTagsAsEmoji()
    {
        var tagString = "";

        var seasonEmojis = new string[] {"🌸", "☀️", "🍂", "❄️"};
        //earthquake
        //tsunami
        //typhoon
        //wildfire
        //flashflood
        //heavysnow
        var disasterEmojis = new string[] { "⛰️", "🌊", "🌀", "🔥", "🌧️", "🌨️" };


        if (SeasonFlags.HasFlag(SeasonsEnum.Spring))
        {
            tagString += seasonEmojis[0];
        }

        if (SeasonFlags.HasFlag(SeasonsEnum.Summer))
        {
            tagString += seasonEmojis[1];
        }

        if (SeasonFlags.HasFlag(SeasonsEnum.Fall))
        {
            tagString += seasonEmojis[2];
        }

        if (SeasonFlags.HasFlag(SeasonsEnum.Winter))
        {
            tagString += seasonEmojis[3];
        }

        if (DisasterFlags.HasFlag(DisastersEnum.Earthquake))
        {
            tagString += disasterEmojis[0];
        }

        if (DisasterFlags.HasFlag(DisastersEnum.Tsunami))
        {
            tagString += disasterEmojis[1];
        }

        if (DisasterFlags.HasFlag(DisastersEnum.Typhoon))
        {
            tagString += disasterEmojis[2];
        }

        if (DisasterFlags.HasFlag(DisastersEnum.WildFire))
        {
            tagString += disasterEmojis[3];
        }

        if (DisasterFlags.HasFlag(DisastersEnum.FlashFlood))
        {
            tagString += disasterEmojis[4];
        }

        if (DisasterFlags.HasFlag(DisastersEnum.HeavySnow))
        {
            tagString += disasterEmojis[5];
        }

        return tagString;
    }


}
