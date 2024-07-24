using Godot;
using System;

public partial class Fireworks : Control
{
	[Export] Texture2D fireworkTex1;

	[Export] Texture2D fireworkTex2;

	[Export] Texture2D fireworkTex3;

	Texture2D[] fireworks;

	Random rnd;

	[Export] int timeBetween = 100;

	int timer = 0;
    public override void _Ready()
    {
		fireworks = new Texture2D[3];
		fireworks[0] = fireworkTex1;
		fireworks[1] = fireworkTex2;
		fireworks[2] = fireworkTex3;

		rnd = new Random();
    }
    public override void _Process(double delta)
	{
		timer -= 1;
		if(timer <= 0) {
			timer = timeBetween;
			shootFirework();
		}
	}

	private void shootFirework()
	{
		// create texture rect
		Sprite2D firework = new Sprite2D();
		AddChild(firework);

		// randomize position
		int x = rnd.Next(5,256);
		int y = rnd.Next(5,100);
		// firework.SetPosition(new Vector2(x,y));
		firework.Position = new Vector2(x,y);

		// randomize texture
		firework.Texture = fireworks[rnd.Next(0, 3)];
		firework.Scale = Vector2.Zero;
		// animate
		Tween tween = firework.CreateTween();
		tween.TweenInterval(0.1);
		tween.SetEase(Tween.EaseType.Out);
		tween.TweenProperty(firework, "scale", Vector2.One, 0.5f);
		tween.TweenProperty(firework, "modulate", new Color("ffffff00"), 1f);
		tween.TweenCallback(Callable.From(firework.QueueFree));
		
	}
}
