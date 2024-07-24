using Godot;
using System;

public partial class counter : StaticBody2D
{
    private CounterArea counterArea;
    private ItemSpawner itemSpawner;

    [Signal]
    public delegate void TakeItemresEventHandler();


    public override void _Ready()
    {
        counterArea = GetNode<CounterArea>("counter_area");
        //Item Spawner for the counter
        itemSpawner = GetNode<ItemSpawner>("ItemSpawner");

        var sceneManager = GetNode<SceneManager>("/root/SceneManager");
        sceneManager.SceneChanged += OnSceneChanged;
    }
    public override void _UnhandledInput(InputEvent @event)
    {
        if(@event.IsActionPressed("Interact"))
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
                    if (!Player._itemSpawner.HasItem())
                    {
                        //give player our itemres
                        Player._itemSpawner.AddItemRes(itemSpawner.RemoveItemRes());
                    }
                    else if (!Player._itemSpawner2.HasItem() && Player.IsStackItemUpgrade)
                    {
                        Player._itemSpawner2.AddItemRes(itemSpawner.RemoveItemRes());
                    }
                } 
                else
                {
                    //attempt to take item from player
                    if (Player._itemSpawner2.HasItem() && Player.IsStackItemUpgrade)
                    {
                        //take itemres from player
                        itemSpawner.AddItemRes(Player._itemSpawner2.RemoveItemRes());
                        EmitSignal(nameof(TakeItemres));
                    }
                    else if (Player._itemSpawner.HasItem())
                    {
                        itemSpawner.AddItemRes(Player._itemSpawner.RemoveItemRes());
                    }
                }
            }
        }
    }

    private void OnSceneChanged(string sceneName)
    {
        if(sceneName.Equals("endofdayscene"))
        {
            if (itemSpawner.HasItem())
            {
                globals.IncrementItemResStock(itemSpawner.RemoveItemRes());
            }
        }
    }

    public Vector2 GetMarkerPosition()
    {
        return GetNode<Marker2D>("Marker2D").GlobalPosition;
    }
}
