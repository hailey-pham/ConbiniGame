using Godot;
using System;

public partial class ItemRes : Resource
{
	[Export] public String name;
	[Export] public int price;
	[Export] public int restockPrice;

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
