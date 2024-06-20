using Godot;
using System.Collections.Generic;

public partial class npc : CharacterBody2D
{

    [Export]
    public float _clusterRadius = 10f;

    [Export]
    public int _maxCounters = 3;


    //will be used to let the counter know this NPC wants to check out
    [Signal]
    public delegate void NPCCheckoutEventHandler(npc npc);

    private Node2D _checkout;

    private int _counterNum = 0;

    private static RandomNumberGenerator rng = new RandomNumberGenerator();

    private NavigationAgent2D _navigationAgent;
    private Timer _timer;

    private float _movementSpeed = 50.0f;

    private List<Vector2> _movementTargets = new List<Vector2>();
    private int _currentTargetIdx = 0;

    private bool leaving = false;

    enum STATE
    {
        shopping,
        checkout,
        leaving
    }

    private STATE _state = STATE.shopping;

    public Vector2 MovementTarget
    {
        get { return _navigationAgent.TargetPosition; }
        set { _navigationAgent.TargetPosition = value; }
    }

    public override void _Ready()
    {
        base._Ready();

        _navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");

        _timer = GetNode<Timer>("Timer");

        //get a reference to our checkout counter when we spawn
        _checkout = (Node2D) GetTree().GetFirstNodeInGroup("checkout");

        //link our signal to the callback in checkout
        checkout checkoutScript = _checkout as checkout;
        NPCCheckout += checkoutScript.OnNPCCheckout;

        _timer.Timeout += OnTimerTimeout;

        foreach (Node counter in GetTree().GetNodesInGroup("counters"))
        {
            Marker2D marker2D = counter.GetNode<Marker2D>("Marker2D");

            if (marker2D != null)
            {
                _movementTargets.Add(marker2D.GlobalPosition);
            }
        }

        //pick a random target to go to
        _currentTargetIdx = rng.RandiRange(0, _movementTargets.Count-1);

        //keep track of how many counters we've been to so far
        _counterNum++;
            

        // These values need to be adjusted for the actor's speed
        // and the navigation layout.
        _navigationAgent.PathDesiredDistance = 4.0f;
        _navigationAgent.TargetDesiredDistance = 4.0f;

        // Make sure to not await during _Ready.
        Callable.From(ActorSetup).CallDeferred();

        if ((bool) _navigationAgent.Get("avoidance_enabled"))
        {
            //register the nav agent to the navserver for avoidance
            NavigationServer2D.AgentSetAvoidanceCallback(_navigationAgent.GetRid(), new Callable(this, MethodName.AvoidanceCompleted));
        }
        
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (_navigationAgent.IsNavigationFinished())
        {
           
            //if we've made it back to the front of the store, die
            if(leaving)
            {
                QueueFree();
            }

            //emit the signal for the counter to pick up
            EmitSignal(nameof(NPCCheckout), this);

            if (_timer.TimeLeft == 0)
            {
                _timer.WaitTime = rng.RandiRange(3, 8);
                _timer.Start();
            }

            Velocity = Vector2.Zero;
            return;
        }

        Vector2 currentAgentPosition = GlobalTransform.Origin;
        Vector2 nextPathPosition = _navigationAgent.GetNextPathPosition();

        //calculate our velocity in a temp variable
        Vector2 vel = currentAgentPosition.DirectionTo(nextPathPosition) * _movementSpeed;

        //if we have disabled avoidance, change velocity and do the move and slide immediately
        if (!(bool)_navigationAgent.Get("avoidance_enabled"))
        {
            Velocity = vel;
            MoveAndSlide();
        }
        else
        {
            //add our velocity to our nav agent for avoidance calculation
            _navigationAgent.Velocity = vel;
        }
    }

    private async void ActorSetup()
    {
        // Wait for the first physics frame so the NavigationServer can sync.
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        // Now that the navigation map is no longer empty, set the movement target.
        if (_movementTargets.Count != 0)
        {
            SetMovementTargetWithRadius(_movementTargets[_currentTargetIdx], _clusterRadius);
        }
    }

    private void SetMovementTargetWithRadius(Vector2 target, float radius)
    {
        MovementTarget = target + new Vector2(rng.RandfRange(-1, 1),rng.RandfRange(-1, 1)).Normalized() * radius;
    }

    //does the actual obstacle avoidance
    private void AvoidanceCompleted(Vector3 newVelocity)
    {
        //now that the avoidance velocity has been calculated, apply the corrected velocity and move and slide
        Velocity = new Vector2(newVelocity.X, newVelocity.Z);
        MoveAndSlide();
    }

    public void OnTimerTimeout()
    {
        if (_counterNum <= _maxCounters) {

            _counterNum++;

            //a simple but effective way to prevevnt picking the same table twice

            var newTargetIdx = _currentTargetIdx;

            while (newTargetIdx == _currentTargetIdx)
            {
                newTargetIdx = rng.RandiRange(0, _movementTargets.Count - 1);
            }

            _currentTargetIdx = newTargetIdx;

            if (_currentTargetIdx >= _movementTargets.Count)
            {
                _currentTargetIdx = 0;
            }

            SetMovementTargetWithRadius(_movementTargets[_currentTargetIdx], _clusterRadius);
        }
        else
        {
            //tell NPC to wait for checkout
            Marker2D marker = GetTree().GetFirstNodeInGroup("checkout").GetNode<Marker2D>("Marker2D");
            MovementTarget = marker.GlobalPosition;
        }
    }

    public void LeaveStore()
    {
        MovementTarget = GetParent<Node2D>().Position;
        leaving = true;
    }
}