using Godot;
using Godot.Collections;
using System;

public partial class LeaveState : State
{
    private npc npcScript;

    private Node2D npcSpawner;
    public override void _Enter(Dictionary message)
    {
        //check for reference to parent
        if (message != null)
        {
            Variant temp;
            if (message.TryGetValue("npcScript", out temp))
            {
                //set our npcBody reference to what was passed in the message dictionary
                npcScript = (npc)temp;
            }
        } else
        {
            npcScript = GetGrandparent() as npc;
        }

        npcScript._navigationAgent.NavigationFinished += OnNavigationFinished;

        npcSpawner = (npcSpawner) GetTree().GetFirstNodeInGroup("spawner");

        npcScript.MoveToPositionOffset(npcSpawner.GlobalPosition);
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

    public void OnNavigationFinished()
    {
        //once we make it out the door, kill ourselves
        npcScript.OnLeaveStore();
        var speechBubble = GetSpeechBubble();
        //if there's no speechBubble, no need to wait
        if (speechBubble == null)
        {
            npcScript.QueueFree();
        }
        else
        {
            //wait for the speech bubble animation to finish and then leave
            var animationPlayer = speechBubble.GetNode<AnimationPlayer>("AnimationPlayer");
            animationPlayer.AnimationFinished += (StringName animName) =>
            {
                npcScript.QueueFree();
            };
        }
    }

    private SpeechBubbleAnimation GetSpeechBubble()
    {
        return npcScript.GetNodeOrNull<SpeechBubbleAnimation>("BubbleSpawn/SpeechBubbleAnimation");
    }
}
