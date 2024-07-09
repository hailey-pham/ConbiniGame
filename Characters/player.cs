using Godot;
using System;
using System.Reflection;

public partial class player : CharacterBody2D
{
	public AnimatedSprite2D _animatedSprite;
	public ItemSpawner _itemSpawner;

	public bool IsPlayerFrozen = false;
    public const float Speed = 100.0f;

    public override void _Ready()
    {
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_itemSpawner = GetNode<ItemSpawner>("ItemSpawner");

		//set the player's held item to a water bottle by default (for testing)
		var globals = GetNode<globals>("/root/Globals");
    }

    public override void _PhysicsProcess(double delta)
	{
		if (!IsPlayerFrozen)
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
    public void playerFreezeState()
    {
		//call this method to stop all player movements or allow the player to move after you stopped them.
		if (IsPlayerFrozen)
		{
			IsPlayerFrozen = false;
		}
		else
		{
            IsPlayerFrozen = true;
			Velocity = Vector2.Zero;
        }

    }

}
