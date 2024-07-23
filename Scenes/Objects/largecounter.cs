using Godot;
using System;

public partial class largecounter : counter
{
    private CounterArea counterArea;
    private ItemSpawner itemSpawner;

    public override void _Ready()
    {
        base._Ready();
    }
    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
        
    }

    private void OnSceneChanged(string sceneName)
    {
        base.OnSceneChanged(sceneName);
    }

    public Vector2 GetMarkerPosition()
    {
        return GetNode<Marker2D>("Marker2D").GlobalPosition;
    }

}
