using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class checkout : StaticBody2D
{
	//the animation that will play when an item is sold
	[Export]
	private PackedScene sellAnimation;

	Timer Timer;
	Area2D PlayerCheck;

	Queue<npc> npcs = new Queue<npc>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PlayerCheck = GetNode<Area2D>("PlayerCheck");
		Timer = GetNode<Timer>("Timer");

		Timer.Timeout += OnTimerTimeout;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//if player is standing at the counter...
		if (PlayerCheck.HasOverlappingBodies())
		{
			Timer.Paused = false;
		} else
		{
			Timer.Paused = true;
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
            var npc = npcs.Dequeue();
			npc.ShoppingCart.ForEach(SellItem);
            npc.stateMachine.TransitionTo("LeaveState");
        }

        Timer.Start();
    }

	public void SellItem(ItemRes item)
	{
        var anim = sellAnimation.Instantiate();
        var animScript = anim as sell_animation;
		animScript.Value = item.price;
		globals.Money += item.price;
		AddChild(anim);
    }
}
