using Godot;
using System;

public partial class counter : StaticBody2D
{
    //takes a signal from the player and then gives that signal to the Area2d counter :3
    [Signal]
    public delegate void TransferSignalEventHandler();
    public void _on_player_hit()
    {
        EmitSignal("TransferSignal");
    }
}
