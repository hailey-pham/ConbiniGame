using Godot;
using System;
using System.Collections.Generic;
using System.Xml.Schema;

public partial class ItemSpawner : Node2D
{
    [Export]
    public PackedScene ItemScene;

    public ItemRes currItem;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

    }

    public bool HasItem()
    {
        if (currItem == null)
        {
            return false;
        }

        return true;
    }

    private void Spawn()
    {
        if (ItemScene != null)
        {
            Node ItemBox = ItemScene.Instantiate();

            //set the item to whatever our current item is
            Item itemScript = ItemBox as Item;
            itemScript.itemRes = currItem;

            AddChild(ItemBox);
        }
    }

    private void Despawn()
    {
        foreach (var child in GetChildren()) {
            child.QueueFree();
        }
    }

    //lets hope this passes by value and not by reference
    public ItemRes RemoveItemRes()
    {
        var item = currItem;
        currItem = null;
        Despawn();
        return item;
    }

    public void AddItemRes(ItemRes item)
    {
        currItem = item;
        Spawn();
    }
}
