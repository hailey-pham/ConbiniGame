using Godot;
using System;
using System.Reflection;

public partial class player : CharacterBody2D
{
	//a reference to the item resource held by the player
	private ItemRes heldItem;

	public bool playerHasObject;
	bool objectIsSpawned;
	public AnimatedSprite2D _animatedSprite;

    [Signal]
    public delegate void HitEventHandler();
	[Signal]
	public delegate void ItemHoldEventHandler();

    [Signal]
    public delegate void InformSpawnEventHandler();
    [Signal]
    public delegate void DeleteEventHandler();

    public const float Speed = 100.0f;

    public override void _Ready()
    {
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		playerHasObject = true;
		objectIsSpawned = false;

		//set the player's held item to a water bottle by default (for testing)
		var globals = GetNode<globals>("/root/Globals");
		heldItem = globals._stock["Water"];
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


		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			var collision = GetSlideCollision(i);
			var body = collision.GetCollider();
			if (body.GetType() == typeof(Area2D))
			{
				EmitSignal(SignalName.Hit);
			}
		}

		
		//If player should have the object and it hasn't spawned, it will spawn.
		//If the palyer shoudn't have the object and it has spawned, then it will despawn
        if (playerHasObject == true && objectIsSpawned == false)
        {
			objectIsSpawned = true;
            EmitSignal("InformSpawn");
        }
		else if (playerHasObject == false && objectIsSpawned == true)
		{
            objectIsSpawned = false;
            EmitSignal("Delete");
        }

       
    }
    //Store inventory signals
    public void _on_inventory_item_clicked(long index, Vector2 at_position, long mouse_button_index)
    {
		playerHasObject = true;
		objectIsSpawned = false;
    }

}
