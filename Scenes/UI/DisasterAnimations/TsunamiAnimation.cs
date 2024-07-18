using Godot;
using System;

public partial class TsunamiAnimation : Control
{
    [Export]
    private float waveSpeed = 1f;

    [Export]
    private float wavePhase = 0f;

    [Export]
    private float waveRadius = 20f;

    [Export]
    private Control waveBg;

    [Export]
    private Control waveFg;

    private Vector2 waveBgPosition;
    private Vector2 waveFgPosition;

    private float waveAngle = 0f;
    public override void _Ready()
    {
        waveBgPosition = waveBg.Position;
        waveFgPosition = waveFg.Position;
    }
    public override void _Process(double delta)
    {
        waveAngle += (float) delta * waveSpeed;
        waveBg.Position = waveBgPosition + new Vector2(Mathf.Cos(waveAngle + wavePhase),Mathf.Sin(waveAngle + wavePhase)) * waveRadius;
        waveFg.Position = waveFgPosition + new Vector2(Mathf.Cos(waveAngle + wavePhase + Mathf.Pi), Mathf.Sin(waveAngle + wavePhase + Mathf.Pi)) * waveRadius;
    }
}
