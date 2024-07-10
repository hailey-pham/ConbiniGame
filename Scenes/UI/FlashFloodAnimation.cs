using Godot;
using System;

public partial class FlashFloodAnimation : Control
{
    [Export]
    private float cloudSpeed = -1f;

    private AnimationPlayer animPlayer;
    private TextureRect clouds;
    private NoiseTexture2D cloudTexture;
    private AudioStreamPlayer thunderSound;
    private AudioStreamPlayer rainSound;
    private WarningAnimation warningAnimation;
    private Timer timer;

    private RandomNumberGenerator rng = new();

    public override void _Ready()
    {
        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        clouds = GetNode<TextureRect>("Clouds");
        thunderSound = GetNode<AudioStreamPlayer>("Thunder");
        rainSound = GetNode<AudioStreamPlayer>("Rain");
        warningAnimation = GetNode<WarningAnimation>("WarningAnimation");
        timer = GetNode<Timer>("Timer");

        warningAnimation.SetDisasterType(Calendar.DisastersEnum.FlashFlood);

        timer.WaitTime = rng.RandfRange(6f, 15f);
        timer.Timeout += OnTimerTimeout;
        timer.Start();

        rainSound.Finished += () =>
        {
            rainSound.Play();
        };
    }

    public override void _Process(double delta)
    {
        cloudTexture ??= (NoiseTexture2D) clouds.Texture;

        Vector3 offset = (Vector3) cloudTexture.Noise.Get("offset");
        cloudTexture.Noise.Set("offset", offset + Vector3.Down * cloudSpeed);
    }

    private async void OnTimerTimeout()
    {
        animPlayer.Play("lightning");
        await ToSignal(animPlayer, "animation_finished");
        thunderSound.Play();
        timer.WaitTime = rng.RandfRange(6f, 15f);
    }
}
