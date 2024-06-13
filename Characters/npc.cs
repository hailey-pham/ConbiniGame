using Godot;
using System.Collections.Generic;

public partial class npc : CharacterBody2D
{
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
            

        // These values need to be adjusted for the actor's speed
        // and the navigation layout.
        _navigationAgent.PathDesiredDistance = 4.0f;
        _navigationAgent.TargetDesiredDistance = 4.0f;

        // Make sure to not await during _Ready.
        Callable.From(ActorSetup).CallDeferred();

        //register the nav agent to the navserver for avoidance
        NavigationServer2D.AgentSetAvoidanceCallback(_navigationAgent.GetRid(), new Callable(this, MethodName.AvoidanceCompleted));
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (_navigationAgent.IsNavigationFinished())
        {
            if (_timer.TimeLeft == 0)
            {
                RandomNumberGenerator rng = new RandomNumberGenerator();
                _timer.WaitTime = rng.RandiRange(3, 8);
                _timer.Start();
            }

            Velocity = Vector2.Zero;
            return;
        }

        Vector2 currentAgentPosition = GlobalTransform.Origin;
        Vector2 nextPathPosition = _navigationAgent.GetNextPathPosition();

        _navigationAgent.Velocity = currentAgentPosition.DirectionTo(nextPathPosition) * _movementSpeed;
    }

    private async void ActorSetup()
    {
        // Wait for the first physics frame so the NavigationServer can sync.
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        // Now that the navigation map is no longer empty, set the movement target.
        if (_movementTargets.Count != 0)
        {
            MovementTarget = _movementTargets[_currentTargetIdx];
        }
    }

    //does the actual obstacle avoidance
    private void AvoidanceCompleted(Vector3 newVelocity)
    {
        Velocity = new Vector2(newVelocity.X, newVelocity.Z);
        MoveAndSlide();
    }

    public void OnTimerTimeout()
    {
        {
            _currentTargetIdx += 1;

            if (_currentTargetIdx >= _movementTargets.Count)
            {
                _currentTargetIdx = 0;
            }

            MovementTarget = _movementTargets[_currentTargetIdx];
        };
    }
}