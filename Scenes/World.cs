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
    public bool bothCounterUpgrades;

    Label dayLabel;
	public override void _Ready()
	{
		counterState = 0;
		bothCounterUpgrades = false;
        calendar = GetNode<Calendar>("/root/Calendar");
		calendarUI = GetNode<CalendarUI>("UI/CalendarUI");
		dayLabel = GetNode<Label>("UI/DayLabel");
		audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		audioPlayer.Finished += () =>
		{
			audioPlayer.Play();
		};

		globals = GetNode<globals>("/root/Globals");

		calendarUI.switchTexture(calendar.GetCurrentSeason());
		dayLabel.Text = calendar.CurrentDay.ToString();


        foreach (KeyValuePair<string, Upgrade> upgrade in globals.Upgrades)
		{
			if (upgrade.Value.owned)
			{
				upgrade.Value.onLevelLoad(globals, GetNode<Node>("."));
            }
        }


		if(bothCounterUpgrades)
		{
			counterState = 2;
		}
        Node2D currCounter = (Node2D)GetTree().GetFirstNodeInGroup("counterstate" + counterState.ToString());
        switch (counterState)
        {
            case 0:
                // code block
                DestroyCounterState(1);
                DestroyCounterState(2);
				currCounter.Visible = true;
                break;
            case 1:
				// code block
				DestroyCounterState(0);
				DestroyCounterState(2);
                currCounter.Visible = true;
                break;
            case 2:
                // code block
                DestroyCounterState(0);
				DestroyCounterState(1);
                currCounter.Visible = true;
                break;
        }
		GetChild<NavigationRegion2D>(0).BakeNavigationPolygon();


	}
    void DestroyCounterState(int state)
    {
		GetTree().GetFirstNodeInGroup("counterstate" + state.ToString()).QueueFree();

    }
}

