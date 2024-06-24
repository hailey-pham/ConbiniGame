using Godot;
using System;

public partial class npcSpawner : Node2D
{
	[Export]
	public PackedScene npcScene;

	[Export]
	public Curve spawnCurve;

	private RandomNumberGenerator random = new RandomNumberGenerator();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_calendar_day_changed()
    {
    }

    private void SpawnNPC()
    {
        if (npcScene != null)
        {
            Node newNpc = npcScene.Instantiate();
            newNpc.Set("z_as_relative", false);
            AddChild(newNpc);
        }
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
}
