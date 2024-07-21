using Godot;
using System;
using System.Collections.Generic;

public partial class npcSpawner : Node2D
{
	[Export]
	public PackedScene npcScene;

	[Export]
	public Curve spawnCurve;

	[Signal]
	public delegate void StoreEmptyEventHandler();

	private AudioStreamPlayer2D audioPlayer;

	private RandomNumberGenerator random = new RandomNumberGenerator();

	private static int npcCount = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//get child node audio stream player 2d
		audioPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");

        //connects NPC spawner to spawn npcs when day increments a percent
        Calendar calendar = GetNode<Calendar>("/root/Calendar");
        calendar.DayPercent += _on_calendar_day_percent;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    private void SpawnNPC()
    {
        if (npcScene != null)
        {
            Node newNpc = npcScene.Instantiate();
            newNpc.Set("z_as_relative", false);
            var npcScript = newNpc as npc;
            npcScript.LeftStore += OnNPCLeaveStore;
            AddChild(newNpc);
			if(audioPlayer.IsInsideTree())
			{
                audioPlayer.Play();
            }
        }
		npcCount++;
    }

    public void _on_calendar_day_percent(int percent)
	{
		var spawnProbability = spawnCurve.Sample(percent/100f);
		var randomNum = random.Randf();

		if(spawnProbability > randomNum)
		{
			SpawnNPC();
		}
	}

	private void OnNPCLeaveStore()
	{
		npcCount--;

		if(IsStoreEmpty())
		{
			//we close the store if the day is up
			var calendar = GetNode<Calendar>("/root/Calendar");
			if(calendar.IsDayOver())
			{
				calendar.EndDay();
			}
		}
	}

	public static bool IsStoreEmpty()
	{
		return npcCount == 0;
	}
}
