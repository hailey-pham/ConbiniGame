using Godot;
using Godot.Collections;
using System;

public abstract partial class State : Node
{

	public StateMachine stateMachine;

	//receives events from the '_unhandled_input()' callback
	abstract public void _Handle_Input(InputEvent input);
	//Corresponds to the '_process()' callback.
	abstract public void _Update(double delta);
	//Corresponds to the '_physics_process()' callback
	abstract public void _Physics_Update(double delta);
	//Called by the state machine upon changing the active state. The 'message'
	//parameter is a dictionary with arbitrary data the state can use to initialize itself
	public void _Enter()
	{
		_Enter(null);
	}
	//Called by state machine when entering new state
	abstract public void _Enter(Dictionary message);
	//Called by the state machine before changing the active state. Use this function to clean up the state.
	abstract public void _Exit();

	public Node GetGrandparent()
	{
		return GetParent().GetParent();
	}
}
