using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class ShoppingState : State
{
    [Export]
    private float buyProbability = 0.5f;

    [Export]
    public int _maxCounters = 3;

    [Export]
    public Timer timer;

    //a reference to the thought bubble animation scenes
    [Export]
    public PackedScene WantItemScene;

    //a reference to the marker where the bubble scene will be spawned in
    private Marker2D bubbleSpawn;

    //saves positions and counters
    private List<Vector2> _movementTargets = new List<Vector2>();
    private List<counter> _counters = new List<counter>();

    private int _counterNum = 0;
    private int _currentTargetIdx = 0;

    private NPCPreferenceModifier modifier;
    private npc npcScript;
    private Dictionary message;

    private static RandomNumberGenerator rng = new RandomNumberGenerator();
    public override void _Enter(Dictionary message)
    {
        //save a copy of our message for later
        this.message = message;

        //check for reference to parent
        if (message != null)
        {
            Variant temp;
            if (message.TryGetValue("npcScript", out temp))
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
                _counters.Add(counter as counter);
            }
        }

        //pick a random target to go to
        _currentTargetIdx = rng.RandiRange(0, _movementTargets.Count - 1);

        //keep track of how many counters we've been to so far
        _counterNum++;

        //tell npc to move to a target position
        npcScript.MoveToPositionOffset(_movementTargets[_currentTargetIdx]);
        npcScript._navigationAgent.NavigationFinished += OnNavigationFinished;

        //get a reference to where we want our speech bubble to spawn
        bubbleSpawn = npcScript.GetNode<Marker2D>("BubbleSpawn");
    }

    
    public override void _Exit()
    {
        //unsubscribe from callback
        npcScript._navigationAgent.NavigationFinished -= OnNavigationFinished;
    }

    public override void _Handle_Input(InputEvent input)
    {

    }


    public void OnNavigationFinished()
    {
        timer.Start();
    }
    public override void _Physics_Update(double delta)
    {

    }

    public override void _Update(double delta)
    {

    }

    private void Timer_Timeout()
    {
        //Gets a reference to the counter, its area2d code with the item logic and the item spawner itself.
        var thisCounter = _counters[_currentTargetIdx];
        var thisItemSpawner = thisCounter.GetNode<ItemSpawner>("ItemSpawner"); // Technically not needed anymore, maybe idk :3

        //check if there is an item to buy and that we want to buy
        if (thisItemSpawner.currItem != null)
        {
            modifier ??= GetNode<NPCPreferenceModifier>("/root/NpcPreferenceModifier");
            ItemRes currItem = thisItemSpawner.currItem;
            buyProbability = modifier.ItemBuyProbability(currItem);
            if(rng.Randf() < buyProbability)
            {
                npcScript.ShoppingCart.Add(thisItemSpawner.RemoveItemRes());
            }
            else
            {
                SpeechBubbleAnimation bubbleAnim = (SpeechBubbleAnimation) WantItemScene.Instantiate();
                bubbleSpawn.AddChild(bubbleAnim);
            }
        }

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
            if (npcScript.ShoppingCart.Count > 0)
            {
                //we're done browsing, so let's now checkout and pass the npcScript through our copied message
                stateMachine.TransitionTo("CheckoutState", message);
            }
            else
            {
                stateMachine.TransitionTo("LeaveState");
            }
            
        }
    }
}
