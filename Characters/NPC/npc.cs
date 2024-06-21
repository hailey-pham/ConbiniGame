using Godot;
using Godot.Collections;

public partial class npc : CharacterBody2D
{
    //handles movement and calling state manager stuff

    [Export]
    private float speed = 50f;

    //spaces the NPCs more around their target destinations
    [Export]
    private float spreadFactor;

    private RandomNumberGenerator rng = new RandomNumberGenerator();

    public StateMachine stateMachine;

    public NavigationAgent2D _navigationAgent;

    private Vector2 _movementTargetPosition = new Vector2(70.0f, 226.0f);

    public Vector2 MovementTarget
    {
        get { return _navigationAgent.TargetPosition; }
        set { _navigationAgent.TargetPosition = value; }
    }
    public override void _Ready()
    {
        //get references to all our component nodes
        _navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
        stateMachine = GetNode("StateMachine") as StateMachine;

        Callable.From(ActorSetup).CallDeferred();
    }

    public override void _Process(double delta)
    {
        
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_navigationAgent.IsNavigationFinished())
        {
            Velocity = Vector2.Zero;
            return;
        }

        //actually move the dude
        Vector2 currentAgentPosition = GlobalTransform.Origin;
        Vector2 nextPathPosition = _navigationAgent.GetNextPathPosition();

        Velocity = speed * currentAgentPosition.DirectionTo(nextPathPosition);

        MoveAndSlide();
    }

    public void MoveToPositionOffset(Vector2 pos)
    {
        //gives a random offset vector
        Vector2 offset = new Vector2(rng.RandfRange(-1,1), rng.RandfRange(-1,1)).Normalized() * spreadFactor;

        MovementTarget = pos + offset;
    }

    private async void ActorSetup()
    {
        // Wait for the first physics frame so the NavigationServer can sync.
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        // Now that the navigation map is no longer empty, set the movement target.
        MovementTarget = _movementTargetPosition;

        //now that navigation is online, let's start shopping!
        Dictionary message = new Dictionary
        {
            { "npcScript", this}
        };

        //transition to our shopping state
        stateMachine.TransitionTo("ShoppingState", message);
    }
}