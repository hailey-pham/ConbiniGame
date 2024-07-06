using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

public partial class Inventory : ItemList
{
    private globals globals;
    private ItemSpawner playerItemSpawner;

    private List<ItemRes> inventoryItems = new List<ItemRes>();
    public override void _Ready()
    {
        playerItemSpawner = GetNode<ItemSpawner>("%Player/ItemSpawner");

        ItemClicked += OnItemClicked;
        globals = GetNode<globals>("/root/Globals");
        Hide();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("Open Inventory"))
        { 
            if(this.IsVisibleInTree())
            {
                Clear();
                Hide();
                GD.Print("meowers in the chat rn :3");
            }
            else
            {
                LoadInventory();
                AddItemsToMenu();
                Show();
            }
        }

    }

    //adds items to the inventory list
    private void LoadInventory()
    {
        foreach(ItemRes item in globals.Stock.Values) {
            if (!inventoryItems.Contains(item) && item.currentStock > 0)
                {
                inventoryItems.Add(item);
            }
        }
    }

    private void AddItemsToMenu()
    {
        foreach (ItemRes item in inventoryItems)
        {
            AddItem(item.name + " " + item.currentStock, item.spriteTexture, selectable:true);
        }
    }

    private void OnItemClicked(long index, Vector2 atPosition, long mouseButtonIndex)
    {
        //get item res and give it to player
        var itemRes = inventoryItems[(int) index];

        if (itemRes != null && !playerItemSpawner.HasItem())
        {
            playerItemSpawner.AddItemRes(itemRes);
        }
    }

}

