using Godot;
using System;

public partial class sell_animation : Control
{
    private AnimationPlayer animPlayer;
    private RichTextLabel label;

    private int value = 0;

    public int Value { get => value; set => this.value = value; }

    public override void _Ready()
    {
        label = GetNode<RichTextLabel>("RichTextLabel");
        SetLabel(value);

        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animPlayer.Play("SellAnimation");
        animPlayer.AnimationFinished += (anim) =>
        {
            QueueFree();
        };
    }

    public void SetLabel(int value)
    {
        label.Text = label.Text + value;
    }
}
