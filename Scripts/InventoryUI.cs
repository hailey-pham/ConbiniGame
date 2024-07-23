using Godot;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;

public partial class InventoryUI : Control
{
	[Export] public PackedScene inventoryIcon;
	[Export] private NinePatchRect BG;
    private globals globals;
    private ItemSpawner playerItemSpawner;
    private ItemSpawner playerItemSpawner2;
    private List<ItemRes> inventoryItems = new List<ItemRes>();
	private List<Control> inventoryIcons = new List<Control>();
	private List<Button> inventoryButtons = new List<Button>();
    private player Player;
    public override void _Ready()
    {
        Player = GetTree().GetFirstNodeInGroup("player") as player;
        playerItemSpawner = Player.GetNode<ItemSpawner>("ItemSpawner");
        playerItemSpawner2 = Player.GetNode<ItemSpawner>("ItemSpawner2");

        globals = GetNode<globals>("/root/Globals");
        BG.Visible = false;
		
		LoadInventory();
    }

	public void LoadInventory()
    {
		GD.Print("Loading Inventory");

        foreach(ItemRes item in globals.Stock.Values) {
            // if (!inventoryItems.Contains(item))
            // {
                inventoryItems.Add(item);
            // }
        }

		LoadItemList();

    }

    public void ReloadItemList()
    {
		int counter = 0;
		Node container;
        foreach (ItemRes item in inventoryItems)
		{
			if (item.currentStock <= 0) {
				inventoryIcons[counter].Visible = false;
			} else {
				inventoryIcons[counter].Visible = true;
				container = inventoryButtons[counter].GetChild(0);
				container.GetChild<TextureRect>(0).Texture = item.spriteTexture;
				container.GetChild<Label>(1).Text = item.name;
				container.GetChild<Label>(2).Text = "x"+item.currentStock;
			}
			counter++;
		}

        
    }

	public void LoadItemList()
	{
		Node container;
		int count = 0;
		foreach (ItemRes item in inventoryItems)
        {
			// GD.Print(count+item.name+item.currentStock);
			inventoryIcons.Add((Control)inventoryIcon.Instantiate());
			GetChild<VBoxContainer>(0).AddChild(inventoryIcons[^1]);

			inventoryButtons.Add(inventoryIcons[^1].GetChild<Button>(0));

			container = inventoryButtons[^1].GetChild(0);

			container.GetChild<TextureRect>(0).Texture = item.spriteTexture;
			container.GetChild<Label>(1).Text = item.name;
			container.GetChild<Label>(2).Text = "x"+item.currentStock;
			
			inventoryButtons[^1].Pressed += () =>  OnItemClicked(item.name);
			count++;
        }
		
#if DEBUG
		GetChild<VBoxContainer>(0).AddChild((Control)inventoryIcon.Instantiate());
#endif
		
	}
	private void OnItemClicked(string itemName)
    {
		// GD.Print("Item clicked" +index);
		// var itemRes = inventoryItems[(int)index];
		var itemRes = globals.Stock[itemName];

		if (itemRes != null)
		{
			if (!playerItemSpawner.HasItem())
			{
				playerItemSpawner.AddItemRes(itemRes);
				globals.DecrementItemResStock(itemRes);
				// Player.playerFreezeState();
				// this.Visible = false;
				// BG.Visible = false;
			}
			//false if it doesn't have an item or if it doesn't exist
			else if (!playerItemSpawner2.HasItem() && Player.IsStackItemUpgrade)
			{
				playerItemSpawner2.AddItemRes(itemRes);
				globals.DecrementItemResStock(itemRes);
				// Player.playerFreezeState();
				// this.Visible = false;
				// BG.Visible = false;
			}
			
		}

		ReloadItemList();
        
        
    }

}

