using Godot;
using System;

public partial class MoneyLabel : RichTextLabel
{
    private globals globals;

    public override void _Ready()
    {
        globals = GetNode<globals>("/root/Globals");
        globals.MoneyUpdated += UpdateMoneyLabel;
    }

    public void UpdateMoneyLabel(int money)
    {
        Text = Text.Substring(0,Text.Find(":") + 1) + money;
    }
}
