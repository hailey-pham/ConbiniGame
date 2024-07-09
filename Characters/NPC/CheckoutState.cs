using Godot;
using Godot.Collections;
using System;

public partial class CheckoutState : State
{

    private npc npcScript;

    private Node2D checkoutNode;
    private checkout checkoutScript;
    private Timer leaveEarlyTimer;
    private AnimationPlayer animationPlayer;
    public override void _Enter(Dictionary message)
    {
        //get timer component
        leaveEarlyTimer = GetNode<Timer>("Timer");
        leaveEarlyTimer.Timeout += OnLeaveEarlyTimeout;
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animationPlayer.AnimationFinished += OnFlashFinished;

        //check for reference to parent
        if (message != null)
        {
            Variant temp;
            if (message.TryGetValue("npcScript", out temp))
            {
                //set our npcBody reference to what was passed in the message dictionary
                npcScript = (npc) temp;
                npcScript._navigationAgent.NavigationFinished += OnNavigationFinished;
            }
        }

        //get our checkout counter
        checkoutNode = (Node2D) GetTree().GetFirstNodeInGroup("checkout");
        checkoutScript = checkoutNode as checkout;

        npcScript.MoveToPositionOffset(checkoutNode.Position);
    }

    public override void _Exit()
    {
        //unsubscribe from callback
        npcScript._navigationAgent.NavigationFinished -= OnNavigationFinished;
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

    public void OnNavigationFinished()
    {
        checkoutScript.OnNPCCheckout(npcScript);
        leaveEarlyTimer.Start();
    }

    private void OnLeaveEarlyTimeout()
    {
        leaveEarlyTimer.Stop();
        animationPlayer.Play("flash");
    }

    private void OnFlashFinished(StringName animName)
    {
        stateMachine.TransitionTo("LeaveState");
    }
}
