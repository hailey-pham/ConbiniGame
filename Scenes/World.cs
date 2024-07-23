using Godot;
using System;
using System.Collections.Generic;

public partial class World : Node2D
{
	// Called when the node enters the scene tree for the first time.
	globals globals;
	private AudioStreamPlayer audioPlayer;

	Calendar calendar;
	CalendarUI calendarUI;

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
		dayLabel.Text = globals.Day.ToString();


        foreach (KeyValuePair<string, Upgrade> upgrade in globals.Upgrades)
		{
			if (upgrade.Value.owned)
			{
				upgrade.Value.onLevelLoad(globals, GetNode<Node>("."));
			}
		}
	}
}
