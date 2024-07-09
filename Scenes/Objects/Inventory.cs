using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

public partial class Inventory : ItemList
{
    private globals globals;
    private ItemSpawner playerItemSpawner;

    private List<ItemRes> inventoryItems = new List<ItemRes>();

    private player Player;
    public override void _Ready()
    {
        Player = GetTree().GetFirstNodeInGroup("player") as player;
        playerItemSpawner = Player.GetNode<ItemSpawner>("ItemSpawner");

        ItemClicked += OnItemClicked;
        globals = GetNode<globals>("/root/Globals");
        Hide();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        /* //OLD INVENTORY CODE TO ACCESS ANYWHERE
        if (Input.IsActionJustPressed("Open Inventory"))
        { 
            if(this.IsVisibleInTree())
            {
                Hide();
                GD.Print("meowers in the chat rn :3");
            }
            else
            {
                LoadInventory();
                Show();
            }
        }
        */
    }

    //adds items to the inventory list
    public void LoadInventory()
    {
        inventoryItems.Clear();

        foreach(ItemRes item in globals.Stock.Values) {
            if (!inventoryItems.Contains(item) && item.currentStock > 0)
                {
                inventoryItems.Add(item);
            }
        }

        ReloadItemList();
    }

    public void ReloadItemList()
    {
        Clear();

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
            globals.DecrementItemResStock(itemRes);
            Player.playerFreezeState();
            Hide();
        }

        LoadInventory();
    }

}

