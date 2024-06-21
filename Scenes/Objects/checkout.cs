using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class checkout : StaticBody2D
{
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
            npc.stateMachine.TransitionTo("LeaveState");
            Timer.Start();
        }
    }
}
