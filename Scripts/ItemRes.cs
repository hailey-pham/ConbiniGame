using Godot;
using System;

public partial class ItemRes : Resource
{
	[Export] public String name;
	[Export] public int price;
	[Export] public int restockPrice;

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



}
