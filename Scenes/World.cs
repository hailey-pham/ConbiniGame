using Godot;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

public partial class World : Node2D
{
	// Called when the node enters the scene tree for the first time.
	globals globals;
	private AudioStreamPlayer audioPlayer;

	Calendar calendar;
	CalendarUI calendarUI;

	public int counterState = 0;

	Label dayLabel;
	public override void _Ready()
	{
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

        switch (counterState)
        {
            case 0:
                // code block
                DestroyCounterState(1);
                DestroyCounterState(2);
				break;
            case 1:
				// code block
				DestroyCounterState(0);
				DestroyCounterState(2);
                break;
            case 2:
                // code block
                DestroyCounterState(0);
				DestroyCounterState(1);
                break;
        }



	}
    void DestroyCounterState(int state)
    {
		GetTree().GetFirstNodeInGroup("counterstate" + state.ToString()).QueueFree();

    }
}

