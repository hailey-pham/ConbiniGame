using Godot;
using System;

public partial class HeavySnowAnimation : Control
{
    [Export]
    private float cloudSpeed = -1f;

    private TextureRect clouds;
    private NoiseTexture2D cloudTexture;

    private RandomNumberGenerator rng = new();
    private AudioStreamPlayer player;

    public override void _Ready()
    {
        clouds = GetNode<TextureRect>("Clouds");
        player.Finished += () =>
        {
            player.Play();
        };
    }

    public override void _Process(double delta)
    {
        cloudTexture ??= (NoiseTexture2D)clouds.Texture;

        Vector3 offset = (Vector3)cloudTexture.Noise.Get("offset");
        cloudTexture.Noise.Set("offset", offset + new Vector3(-1,1,0) * cloudSpeed);
    }
}
