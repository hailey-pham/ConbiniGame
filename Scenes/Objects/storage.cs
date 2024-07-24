using Godot;
using System.Collections.Generic;

public partial class storage : Area2D
{
    [Signal]
    public delegate void FreezePlayerEventHandler();

    [Signal]
    public delegate void InventoryOpenedEventHandler();

    public player Player = null;
    private InventoryUI Inventory;

    private NinePatchRect BG;
    //public ItemSpawner playerItemSpawnerParent;

    private List<ItemRes> inventoryItems = new List<ItemRes>();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;

        //unique naming that can access a node anywhere
        //playerItemSpawnerParent = 

        //force self to be visible
        Visible = true;

        Inventory = GetNode<InventoryUI>("Control/InventoryUI");
        BG = GetNode<NinePatchRect>("Control/NinePatchRect");
        Inventory.Visible = false;
        BG.Visible = false;
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
                //if player is currently holding an item
                // if (Player._itemSpawner.currItem != null && (Player._itemSpawner2.currItem != null || !Player.IsStackItemUpgrade))
                // {
                //     if(Player.IsStackItemUpgrade)
                //     {
                //         // Player._itemSpawner2.currItem.ReturnItemToStock();
                        
                //     }
                //     //return it to stock and remove the item
                //     // Player._itemSpawner.currItem.ReturnItemToStock();
                    
                // }
                
                // if (Inventory.IsVisibleInTree())
                if(Inventory.Visible) // when inventory is closed
                {
                    Inventory.Visible = false;
                    BG.Visible = false;
                    Player.playerFreezeState();
                }
                else // when inventory is opened
                {
                    // if (Player._itemSpawner.currItem != null && (Player._itemSpawner2.currItem != null || !Player.IsStackItemUpgrade))
                    if(Player._itemSpawner.currItem != null)
                    {
                        Player._itemSpawner.currItem.ReturnItemToStock();
                        Player._itemSpawner.RemoveItemRes();

                        if(Player.IsStackItemUpgrade && Player._itemSpawner2.currItem != null)
                        { 
                            Player._itemSpawner2.currItem.ReturnItemToStock();
                            Player._itemSpawner2.RemoveItemRes();
                        }
                    }

                    Inventory.ReloadItemList();
                    Inventory.Visible = true;
                    BG.Visible = true;
                    Player.playerFreezeState();
                    EmitSignal(nameof(InventoryOpened));
                }

                
                
            }

        }
    }
}
