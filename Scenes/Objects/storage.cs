using Godot;
using System.Collections.Generic;

public partial class storage : Area2D
{
    [Signal]
    public delegate void FreezePlayerEventHandler();

    public player Player = null;
    private Inventory Inventory;
    //public ItemSpawner playerItemSpawnerParent;

    private List<ItemRes> inventoryItems = new List<ItemRes>();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;

        //unique naming that can access a node anywhere
        //playerItemSpawnerParent = 

        Inventory = GetNode<Inventory>("Inventory");
    }

    public void OnBodyEntered(Node2D body)
    {
        Player = body as player;
    }

    public void OnBodyExited(Node2D body)
    {
        if (body.Equals(Player))
        {
            Player = null;
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("Interact"))
        {
            //player is not null when player is inside area
            
            //basically the same as checking if the player is inside the area
            if (Player != null)
            {
                if (Inventory.IsVisibleInTree())
                {
                    Inventory.Hide();
                }
                else
                {
                    Inventory.LoadInventory();
                    Inventory.Show();
                }

                Player.playerFreezeState();
            }

        }
    }
}
