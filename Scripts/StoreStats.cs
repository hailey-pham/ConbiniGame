using Godot;
using System;

public partial class StoreStats : Node
{
	int money = 0;
	String[] items = {"Water", "Food"};
	int[] itemStock = {0, 0};
	int stock;
	bool[] indoorUpgrades = {};
	bool[] outdoorUpgrades = {};
}
