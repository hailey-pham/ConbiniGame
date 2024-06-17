using Godot;
using System;

public partial class Title : RichTextLabel
{
	int effectIndex = 0;
	[Export]
	String[] prefixes;
	[Export]
	String[] suffixes;

	String title;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		title = Text;
		ApplyTextEffect();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	//change menu effect every time the timer goes off
	public void _on_timer_timeout()
	{
		effectIndex++;
		if (effectIndex == prefixes.Length)
		{
			effectIndex = 0;
		}

		ApplyTextEffect();
    }

	private void ApplyTextEffect()
	{
        Text = prefixes[effectIndex] + title + suffixes[effectIndex];
    }
}
