using Godot;
using System;

public partial class player : CharacterBody2D
{
	public AnimatedSprite2D _animatedSprite;

	public const float Speed = 100.0f;

	enum Directions
	{
		Left,
		Right,
		Up,
		Down,
	}

	private Directions _lastDirection = Directions.Down;

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

		HandleAnimation(direction);
		Velocity = velocity;
		MoveAndSlide();
	}

	private void HandleAnimation(Vector2 direction)
	{
		if (direction == Vector2.Zero)
		{
			switch (_lastDirection)
			{
				case Directions.Left:
					_animatedSprite.Play("idle_left");
					break;
				case Directions.Right:
					_animatedSprite.Play("idle_right");
					break;
				case Directions.Up:
					_animatedSprite.Play("idle_up");
					break;
				case Directions.Down:
					_animatedSprite.Play("idle_down");
					break;
			}
		}
		else
		{
			//diagonal movement
			if (Math.Abs(direction.X) == Math.Abs(direction.Y))
			{
                switch (_lastDirection)
                {
                    case Directions.Left:
                        _animatedSprite.Play("walk_left");
                        break;
                    case Directions.Right:
                        _animatedSprite.Play("walk_right");
                        break;
                    case Directions.Up:
                        _animatedSprite.Play("walk_up");
                        break;
                    case Directions.Down:
                        _animatedSprite.Play("walk_down");
                        break;
                }
            } 
			else if (direction.X > 0)
			{
				_animatedSprite.Play("walk_right");
				_lastDirection = Directions.Right;
			}
            else if (direction.X < 0)
            {
                _animatedSprite.Play("walk_left");
                _lastDirection = Directions.Left;
            }
			else if (direction.Y > 0)
			{
				_animatedSprite.Play("walk_down");
				_lastDirection = Directions.Down;
			} 
			else if (direction.Y < 0)
			{
				_animatedSprite.Play("walk_up");
				_lastDirection = Directions.Up;
			}
        }
	}
}
