using Godot;
using System;

public partial class TyphoonAnimation : Control
{
    [Export]
    private float cloudSpeed = 1f;

    private TextureRect clouds;
    private NoiseTexture2D cloudTexture;
    private AudioStreamPlayer rain;
    public override void _Ready()
    {
        clouds = GetNode<TextureRect>("Clouds");
        rain = GetNode<AudioStreamPlayer>("Rain");
        rain.Finished += () =>
        {
            rain.Play();
        };
    }

    public override void _Process(double delta)
    {
        cloudTexture ??= (NoiseTexture2D)clouds.Texture;

        Vector3 offset = (Vector3)cloudTexture.Noise.Get("offset");
        cloudTexture.Noise.Set("offset", offset + Vector3.Right * (float)(cloudSpeed*delta));
    }
}
