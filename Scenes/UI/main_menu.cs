using Godot;
using System;

public partial class main_menu : Control
{
    private Button playButton;
    private Button creditsButton;
    private Button quitButton;

    private SceneManager sceneManager;

    public override void _Ready()
    {
        //get referenes to all the things instatiated at runtime
        sceneManager = GetNode<SceneManager>("/root/SceneManager");

        var vbox = GetNode<VBoxContainer>("VBoxContainer");
        playButton = vbox.GetNode<Button>("PlayButton");
        creditsButton = vbox.GetNode<Button>("CreditsButton");
        quitButton = vbox.GetNode<Button>("QuitButton");

        playButton.Pressed += onPlayPressed;
        creditsButton.Pressed += onCreditsPressed;
        quitButton.Pressed += onQuitPressed;
    }

    private void onPlayPressed()
    {
        //change scene to game scene
        sceneManager.ChangeScene("gamescene");
    }

    private void onCreditsPressed()
    {
        //show some credits here
        sceneManager.ChangeScene("credits");
    }

    private void onQuitPressed()
    {
        //quit the game
        GetTree().Quit();
    }
}
