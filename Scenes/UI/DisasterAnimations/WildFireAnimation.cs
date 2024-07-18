using Godot;
using System;

public partial class WildFireAnimation : Control
{
    [Export]
    private float cloudSpeed = -1f;

    private TextureRect fire;
    private NoiseTexture2D fireTexture;

    private RandomNumberGenerator rng = new();

    public override void _Ready()
    {
        fire = GetNode<TextureRect>("Fire");
    }

    public override void _Process(double delta)
    {
        fireTexture ??= (NoiseTexture2D)fire.Texture;

        Vector3 offset = (Vector3)fireTexture.Noise.Get("offset");
        fireTexture.Noise.Set("offset", offset + Vector3.Down * cloudSpeed);
    }
}
