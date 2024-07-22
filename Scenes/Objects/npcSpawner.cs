using Godot;
using System;
using System.Collections.Generic;

public partial class npcSpawner : Node2D
{
	[Export]
	private int minNPCsInStore = 1;
	[Export]
	private int maxNPCsInStore = 5;

	[Export]
	private float maxFirstNPCSpawnTime = 5f;

	[Export] 
	public int npcWaitTime = 20;

	[Export]
	public PackedScene npcScene;

	[Export]
	public Curve spawnCurve;

	[Signal]
	public delegate void StoreEmptyEventHandler();

	private AudioStreamPlayer2D audioPlayer;

	private RandomNumberGenerator random = new RandomNumberGenerator();

	private static int npcCount = 0;

	private Calendar calendar;

	private Timer firstNPCTimer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//get child node audio stream player 2d
		audioPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
		firstNPCTimer = GetNode<Timer>("FirstNPCTimer");

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
        if (npcScene != null && npcCount <= maxNPCsInStore)
        {
            Node newNpc = npcScene.Instantiate();
            newNpc.Set("z_as_relative", false);
            var npcScript = newNpc as npc;
            npcScript.LeftStore += OnNPCLeaveStore;
            AddChild(newNpc);
			newNpc.GetNode<Timer>("StateMachine/CheckoutState/Timer").WaitTime = npcWaitTime;
			if(audioPlayer.IsInsideTree())
			{
                audioPlayer.Play();
            }
            npcCount++;
        }

    }

    public void _on_calendar_day_percent(int percent)
    {
        var spawnProbability = spawnCurve.Sample(percent / 100f);
        var randomNum = random.Randf();

        AddNPCIfEmpty();

        if (spawnProbability > randomNum)
        {
            SpawnNPC();
        }
    }

    private void AddNPCIfEmpty()
    {
        if (npcCount == 0 && firstNPCTimer.TimeLeft == 0)
        {
            RandomNumberGenerator rng = new RandomNumberGenerator();
            firstNPCTimer.WaitTime = rng.RandfRange(0, maxFirstNPCSpawnTime);
            firstNPCTimer.Start();
            firstNPCTimer.Timeout += () =>
            {
                SpawnNPC();
                firstNPCTimer.Stop();
            };
        }
    }

    private void OnNPCLeaveStore()
	{
		npcCount--;

		if(IsStoreEmpty())
		{
			//we close the store if the day is up
			calendar ??= GetNode<Calendar>("/root/Calendar");
			if (calendar.IsDayOver())
			{
				calendar.EndDay();
			}
			else
			{
				AddNPCIfEmpty();
			}
		}
	}

	public static bool IsStoreEmpty()
	{
		return npcCount == 0;
	}
}
