using Godot;
using System;

public partial class player : CharacterBody2D
{
	public AnimatedSprite2D _animatedSprite;

	public const float Speed = 100.0f;

    public override void _Ready()
    {
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }
    public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down").Normalized();
		if (direction != Vector2.Zero)
		{
			velocity = direction * Speed;
		}
		else
		{
			velocity = velocity.MoveToward(Vector2.Zero, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
