using Godot;
using System.Collections.Generic;

public partial class npc : CharacterBody2D
{
    private static RandomNumberGenerator rng = new RandomNumberGenerator();

    private NavigationAgent2D _navigationAgent;
    private Timer _timer;

    private float _movementSpeed = 50.0f;
    private bool _isFirstMovementTargetPosition = true;

    private List<Vector2> _movementTargets = new List<Vector2>();
    private int _currentTargetIdx = 0;

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
            SetMovementTargetWithRadius(_movementTargets[_currentTargetIdx], 5f);
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
        {

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

            SetMovementTargetWithRadius(_movementTargets[_currentTargetIdx], 5f);
        };
    }
}