using Godot;
using System;
using System.ComponentModel;
using static Godot.TextServer;

public partial class AutoAnimator : Node
{
    [Export]
    public AnimatedSprite2D _animatedSprite;

    [Export] public float _deadZone = 0.1f;
    enum Directions
    {
        Left,
        Right,
        Up,
        Down,
    }

    private Directions _lastDirection = Directions.Down;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        HandleAnimation(GetParent<CharacterBody2D>().Velocity.Normalized());
    }

    private void HandleAnimation(Vector2 direction)
    {
        if (direction.Length() <= _deadZone)
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
            else if (direction.X > _deadZone)
            {
                _animatedSprite.Play("walk_right");
                _lastDirection = Directions.Right;
            }
            else if (direction.X < -_deadZone)
            {
                _animatedSprite.Play("walk_left");
                _lastDirection = Directions.Left;
            }
            else if (direction.Y > _deadZone)
            {
                _animatedSprite.Play("walk_down");
                _lastDirection = Directions.Down;
            }
            else if (direction.Y < -_deadZone)
            {
                _animatedSprite.Play("walk_up");
                _lastDirection = Directions.Up;
            }
        }
    }
}
