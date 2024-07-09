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

    private bool beingCheckedOut = false;
    public override void _Enter(Dictionary message)
    {
        //get timer component
        leaveEarlyTimer = GetNode<Timer>("Timer");
        leaveEarlyTimer.Timeout += OnLeaveEarlyTimeout;
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animationPlayer.AnimationFinished += OnFlashFinished;

        //offset timer length for animation
        leaveEarlyTimer.WaitTime = leaveEarlyTimer.WaitTime - animationPlayer.GetAnimation("flash").Length;

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
        //stop the red flashing
        animationPlayer.Stop();
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
        if(!beingCheckedOut)
        {
            animationPlayer.Play("flash");
        }
    }

    private void OnFlashFinished(StringName animName)
    {
        if(!beingCheckedOut)
        {
            //let the checkout counter know we're leaving early and unsubscribe us from the npc list
            npcScript.EmitSignal(nameof(npcScript.LeftEarly),npcScript);

            //leave the building
            stateMachine.TransitionTo("LeaveState");
        }
    }

    //these two methods do the opposite of each other
    public void PauseLeaveEarly()
    {
        leaveEarlyTimer.Paused = true;

        if(animationPlayer.IsPlaying())
        {
            animationPlayer.PlayBackwards("flash", animationPlayer.CurrentAnimationPosition);
        }

        beingCheckedOut = true;
    }

    public void ResumeLeaveEarly()
    {
        leaveEarlyTimer.Paused = false;

        if (animationPlayer.IsPlaying())
        {
            animationPlayer.Play("flash", animationPlayer.CurrentAnimationPosition);
        }

        beingCheckedOut = false;
    }
}
