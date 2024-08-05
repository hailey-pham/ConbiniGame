using Godot;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using static Godot.WebSocketPeer;

public partial class World : Node2D
{
	// Called when the node enters the scene tree for the first time.
	globals globals;
	private AudioStreamPlayer audioPlayer;

	Calendar calendar;
	CalendarUI calendarUI;

	public int counterState;
	public int prevCounterState;
    public bool bothCounterUpgrades;
	PackedScene scene;
	Node instance;

    Label dayLabel;

	// Control pauseMenu;
	

    public override void _Ready()
	{
        DestroyCounterState(0);
        DestroyCounterState(1);
        DestroyCounterState(2);
        counterState = 0;
		bothCounterUpgrades = false;
        NavigationRegion2D navMesh = GetNode<NavigationRegion2D>("NavigationRegion2D");


        calendar = GetNode<Calendar>("/root/Calendar");
		calendarUI = GetNode<CalendarUI>("UI/CalendarUI");
		dayLabel = GetNode<Label>("UI/DayLabel");
		audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		// pauseMenu = GetNode<Control>("PauseMenu");
		// pauseMenu.Visible = false;
		audioPlayer.Finished += () =>
		{
			audioPlayer.Play();
		};

		globals = GetNode<globals>("/root/Globals");

		calendarUI.switchTexture(calendar.GetCurrentSeason());
		dayLabel.Text = calendar.CurrentDay.ToString();
		globals.CustomersEntered = 0;


		foreach (KeyValuePair<string, Upgrade> upgrade in globals.Upgrades)
		{
			if (upgrade.Value.owned)
			{
				upgrade.Value.onLevelLoad(globals, GetNode<Node>("."));
			}
		}
		if (bothCounterUpgrades)
		{
			counterState = 2;
		}
        Node2D currCounter = (Node2D)GetTree().GetFirstNodeInGroup("counterstate" + counterState.ToString());
            
        switch (counterState)
        {
            case 0:
                // code block
                scene = GD.Load<PackedScene>("res://Scenes/counter_upgrade_state_0.tscn");
				instance = scene.Instantiate();
				navMesh.AddChild(instance);
                //currCounter.Visible = true;
                break;
            case 1:
                // code block
                scene = GD.Load<PackedScene>("res://Scenes/counter_upgrade_state_1.tscn");
                instance = scene.Instantiate();
                navMesh.AddChild(instance);
                //currCounter.Visible = true;
                break;
            case 2:
                // code block
                scene = GD.Load<PackedScene>("res://Scenes/counter_upgrade_state_2.tscn");
                instance = scene.Instantiate();
                navMesh.AddChild(instance);
                //currCounter.Visible = true;
                break;
        }
        navMesh.BakeNavigationPolygon();
        

		prevCounterState = counterState;
	}
    void DestroyCounterState(int state)
    {
		GetTree().GetFirstNodeInGroup("counterstate" + state.ToString()).QueueFree();

    }

	void LoadCounters()
	{

	}

	// public override void _Input(InputEvent @event)
    // {
    //     if(@event.IsActionReleased("Exit"))
    //     {
	// 		pauseMenu.Visible = !pauseMenu.Visible;
	// 		GetTree().Paused = !GetTree().Paused;
	// 	}
    // }
}

