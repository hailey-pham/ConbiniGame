using Godot;
using System;

public partial class CalendarUI : TextureRect
{
	[Export] Texture2D spring;
	[Export] Texture2D summer;
	[Export] Texture2D fall;
	[Export] Texture2D winter;
	public void switchTexture(int season) {
		switch(season)
		{
			case 1:
				Texture = spring;
				break;
			case 2:
				Texture = summer;
				break;
			case 3:
				Texture = fall;
				break;
			case 4:
				Texture = winter;
				break;
		}

	}
}
