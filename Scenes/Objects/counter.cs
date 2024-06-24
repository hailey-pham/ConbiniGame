using Godot;
using System;

public partial class counter : StaticBody2D
{
    //takes a signal from the player and then gives that signal to the Area2d counter :3
    [Signal]
    public delegate void TransferSignalEventHandler();
    [Signal]
    public delegate void TransferInformSpawnEventHandler();
    [Signal]
    public delegate void TransferDeleteEventHandler();
    public void _on_player_hit()
    {
        EmitSignal("TransferSignal");
    }
    public void _on_area_2d_delete()
    {
        EmitSignal("TransferDelete");
    }
    public void _on_area_2d_inform_spawn()
    {
        EmitSignal("TransferInformSpawn");
    }
}
