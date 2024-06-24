using Godot;
using Godot.Collections;
using System;

public partial class StateMachine : Node
{
	[Signal]
	public delegate void TransitionedEventHandler(String stateName);

	[Export]
	NodePath initialState;

	private State state;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//get initial state at start of game
		state = GetNode(initialState) as State;

		//assign state machine to every state child
		foreach(State child in GetChildren())
		{
			child.stateMachine = this;
		}
		state._Enter();
	}

    public override void _UnhandledInput(InputEvent @event)
    {
		state._Handle_Input(@event);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		state._Update(delta);
	}

    public override void _PhysicsProcess(double delta)
    {
        state._PhysicsProcess(delta);
    }

	public void TransitionTo(String stateName, Dictionary message = null)
	{
		//quit if we don't have node
		if (!HasNode(stateName))
		{
			GD.PrintErr("State not found!");
			return;
		}

		state._Exit();
		state = GetNode(stateName) as State;
		if (message != null)
		{
            state._Enter(message);
        } else
		{
			state._Enter();
		}

		EmitSignal(nameof(Transitioned),state.Name);
	}
}
