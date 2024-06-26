using Godot;
using System;

public partial class Inventory : ItemList
{
    
    public override void _Ready()
    {
        Hide();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("Open Inventory"))
        { 
            if(this.IsVisibleInTree())
            {
                Hide();
                GD.Print("meowers in the chat rn :3");
            }
            else
            {
                Show();
            }
        }

    }

}

