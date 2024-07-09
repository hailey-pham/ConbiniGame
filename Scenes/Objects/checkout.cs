using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class checkout : StaticBody2D
{
	//the animation that will play when an item is sold
	[Export]
	private PackedScene sellAnimation;

	private Timer timer;
	private Area2D playerCheck;
	private AudioStreamPlayer audioStreamPlayer;
	private globals globals;

	private List<npc> npcs = new();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		playerCheck = GetNode<Area2D>("PlayerCheck");
		timer = GetNode<Timer>("Timer");
		audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        globals = GetNode<globals>("/root/Globals");

        timer.Timeout += OnTimerTimeout;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
		if (npcs.Count > 0)
		{
            
            var nextNpc = npcs.First();
            var checkoutState = nextNpc.stateMachine.GetCurrentState() as CheckoutState;

            //if player is standing at the counter and NPC is waiting...
            if (playerCheck.HasOverlappingBodies())
			{
				if(timer.IsStopped())
				{
					timer.Start();
				}

                //if the NPC is getting impatient, tell him to chill out. he's next.
                checkoutState.PauseLeaveEarly();
            }
			else
			{
				//if player is not at counter and NPC is waiting...
				timer.Stop();

				//tell NPC to get impatient again. Player has left him and he should be mad.
				checkoutState.ResumeLeaveEarly();
			}
			
		} else
		{
			//if player is not at counter and NPC isn't waiting
			timer.Stop();
		}
	}

	//called directly by an npc
	public void OnNPCCheckout(npc npc)
	{
		npcs.Add(npc);
		npc.LeftEarly += OnNPCLeaveEarly;
	}

	//the callback we run when an NPC fires LeaveEarly signal
	private void OnNPCLeaveEarly(npc npc)
	{
		if(npcs.Contains(npc))
		{
			npc.ShoppingCart.ForEach(ReturnItemToStock);
			npcs.Remove(npc);
		}
	}

	private void ReturnItemToStock(ItemRes item)
	{
		
		//add one back to the global stock
		globals.Stock[item.name].currentStock++;
	}

	//called when our timer finishes
	public void OnTimerTimeout()
	{
		if (npcs.Count > 0)
		{	
			var npc = npcs.First();
			if (npc.ShoppingCart.Count > 0)
			{
                var item = npc.ShoppingCart[0];
				npc.ShoppingCart.RemoveAt(0);

				SellItem(item);
				if(npc.ShoppingCart.Count == 0)
				{
                    globals.Customers += 1;
                    //we have sold everything in the npc's cart
                    npcs.RemoveAt(0);
                    npc.stateMachine.TransitionTo("LeaveState");
                }
            }
        }

        timer.Start();
    }

	public void SellItem(ItemRes item)
	{
		//animate the items profit
        var anim = sellAnimation.Instantiate();
        var animScript = anim as sell_animation;
		animScript.Value = item.price;
        AddChild(anim);

		//play the sound
		audioStreamPlayer.Play();

		globals.Earnings += item.price;
        globals.Money += item.price;
		
    }
}
