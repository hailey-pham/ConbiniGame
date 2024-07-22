using Godot;
using System;

public partial class GameOver : Control
{
    [Signal] public delegate void ButtonPressedEventHandler();

    private Button contButton;
    private SceneManager sceneManager;
    public override void _Ready()
    {
        contButton = GetNode<Button>("Button");
        sceneManager = GetNode<SceneManager>("/root/SceneManager");
        //Gets a signal from the child and executes the method in the parent.
        contButton.Pressed += OnButtonPressed;
    }
    
    public void OnButtonPressed()
    {
        EmitSignal(nameof(ButtonPressed));
        GD.Print("Continue :3");
        sceneManager.ChangeScene("mainmenu", "FadeToBlack");
    }
}
