using Godot;
using System;

public partial class counter : StaticBody2D
{
    private CounterArea counterArea;
    private ItemSpawner itemSpawner;

    public override void _Ready()
    {
        counterArea = GetNode<CounterArea>("counter_area");
        itemSpawner = GetNode<ItemSpawner>("ItemSpawner");
    }
    public override void _UnhandledInput(InputEvent @event)
    {
        if(@event.IsAction("Interact"))
        {
            //player is not null when player is inside area
            player Player = counterArea.Player;
            //basically the same as checking if the player is inside the area
            if (Player != null)
            {
                if (itemSpawner.HasItem())
                {
                    //attempt to transfer current item to player

                    //if player currently doesn't have an item
                    if(!Player._itemSpawner.HasItem())
                    {
                        //give player our itemres
                        Player._itemSpawner.AddItemRes(itemSpawner.RemoveItemRes());
                    }
                } else
                {
                    //attempt to take item from player
                    if (Player._itemSpawner.HasItem())
                    {
                        //take itemres from player
                        itemSpawner.AddItemRes(Player._itemSpawner.RemoveItemRes());
                    }
                }
            }
        }
    }
}
