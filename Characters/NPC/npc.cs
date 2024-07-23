using ConbiniGame.Scripts;
using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = ConbiniGame.Scripts.Debug;

public partial class npc : CharacterBody2D
{
    //fired when NPC leaves store
    [Signal]
    public delegate void LeftStoreEventHandler();

    //fired when NPC leaves counter early
    [Signal] public delegate void LeftEarlyEventHandler(npc npc);

    //handles movement and calling state manager stuff
    [Export]
    private float speed = 50f;

    //spaces the NPCs more around their target destinations
    [Export]
    private float spreadFactor;

    private RandomNumberGenerator rng = new RandomNumberGenerator();

    public StateMachine stateMachine;

    public NavigationAgent2D _navigationAgent;

    private AnimatedSprite2D _animatedSprite;

    //using a private variable and a public property in case I want to change it down the line
    private List<ItemRes> shoppingCart = new List<ItemRes>();
    public List<ItemRes> ShoppingCart { get => shoppingCart; set => shoppingCart = value; }

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
        _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
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

    public void OnLeaveStore()
    {
        EmitSignal(nameof(LeftStore));
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

    public void SetSpriteFrames(SpriteFrames spriteFrames)
    {
        Debug.Assert(_animatedSprite != null,"Tried to change sprite frames before entering tree!");
        _animatedSprite.SpriteFrames = spriteFrames;
    }
}