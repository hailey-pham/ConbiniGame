using Godot;
using System;

public partial class Victory : Control
{
    [Signal] public delegate void ButtonPressedEventHandler();

    private Button contButton;
    private Button restButton;
    private SceneManager sceneManager;
    public override void _Ready()
    {
        contButton = GetNode<Button>("Button");
        restButton = GetNode<Button>("Button2");
        sceneManager = GetNode<SceneManager>("/root/SceneManager");
        //Gets a signal from the child and executes the method in the parent.
        contButton.Pressed += OnContButtonPressed;
        restButton.Pressed += OnRestButtonPressed;
    }

    public void OnRestButtonPressed()
    {
        EmitSignal(nameof(ButtonPressed));
        GD.Print("Back to Start :3");
        sceneManager.ChangeScene("mainmenu", "FadeToBlack");
    }
    public void OnContButtonPressed()
    {
        EmitSignal(nameof(ButtonPressed));
        GD.Print("Continue :3");
        sceneManager.ChangeScene("gamescene", "FadeToBlack");
    }
}
