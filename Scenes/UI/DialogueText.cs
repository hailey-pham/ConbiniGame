using Godot;
using System;

public partial class DialogueText : RichTextLabel
{
	public void SetDialogueText(string text)
	{
		Text = text;
	}

}
