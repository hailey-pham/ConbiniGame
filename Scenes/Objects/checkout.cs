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

	Queue<npc> npcs = new Queue<npc>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playerCheck = GetNode<Area2D>("PlayerCheck");
		timer = GetNode<Timer>("Timer");
		audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");

		timer.Timeout += OnTimerTimeout;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//if player is standing at the counter and NPC is waiting...
		if (playerCheck.HasOverlappingBodies() && npcs.Count > 0)
		{
			timer.Paused = false;
		} else
		{
			timer.Paused = true;
		}
	}

	//the callback we run when an NPC fires NPCCheckout Signal
	public void OnNPCCheckout(npc npc)
	{
		npcs.Enqueue(npc);
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
					//we have sold everything in the npc's cart
					npcs.Dequeue();
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

		//update global variables
        var globals = GetNode<globals>("/root/Globals");
		globals.Earnings += item.price;
        globals.Money += item.price;
		globals.Customers += 1;
    }
}
