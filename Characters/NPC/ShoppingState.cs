using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class ShoppingState : State
{
    [Export]
    public int _maxCounters = 3;

    [Export]
    public Timer timer;

    private List<Vector2> _movementTargets = new List<Vector2>();

    private int _counterNum = 0;
    private int _currentTargetIdx = 0;

    private npc npcScript;

    private static RandomNumberGenerator rng = new RandomNumberGenerator();
    public override void _Enter(Dictionary message)
    {
        //check for reference to parent
        if (message != null)
        {
            Variant temp;
            if (message.TryGetValue("npcBody", out temp))
            {
                //set our npcBody reference to what was passed in the message dictionary
                npcScript = (npc) temp;
            }
        }

        //link our timer callback to the timeout signal of the timer
        timer.Timeout += Timer_Timeout;

        //get all the counters in the store and save their positions for later
        foreach (Node counter in GetTree().GetNodesInGroup("counters"))
        {
            Marker2D marker2D = counter.GetNode<Marker2D>("Marker2D");

            if (marker2D != null)
            {
                _movementTargets.Add(marker2D.GlobalPosition);
            }
        }

        //pick a random target to go to
        _currentTargetIdx = rng.RandiRange(0, _movementTargets.Count - 1);

        //keep track of how many counters we've been to so far
        _counterNum++;

        //tell npc to move to a target position
        npcScript.MoveToPositionOffset(_movementTargets[_currentTargetIdx]);
    }

    
    public override void _Exit()
    {
        
    }

    public override void _Handle_Input(InputEvent input)
    {

    }

    public override void _Physics_Update(double delta)
    {

    }

    public override void _Update(double delta)
    {

    }

    private void Timer_Timeout()
    {
        if (_counterNum <= _maxCounters)
        {
            _counterNum++;

            //a simple but effective way to prevevnt picking the same table twice

            var newTargetIdx = _currentTargetIdx;

            while (newTargetIdx == _currentTargetIdx)
            {
                newTargetIdx = rng.RandiRange(0, _movementTargets.Count - 1);
            }

            _currentTargetIdx = newTargetIdx;

            npcScript.MoveToPositionOffset(_movementTargets[newTargetIdx]);
        }
        else
        {
            //we're done browsing, so let's now checkout
            stateMachine.TransitionTo("CheckoutState");
        }
    }
}
