using Godot;
using System;

public partial class main_menu : Control
{
    private Button playButton;
    private Button creditsButton;
    private Button quitButton;

    private SceneManager sceneManager;

    private globals globals;

    public override void _Ready()
    {
        //get referenes to all the things instatiated at runtime
        sceneManager = GetNode<SceneManager>("/root/SceneManager");
        globals = GetNode<globals>("/root/Globals");

        var hbox = GetNode<HBoxContainer>("HBoxContainer");
        playButton = hbox.GetNode<Button>("PlayButton");
        creditsButton = hbox.GetNode<Button>("CreditsButton");
        quitButton = hbox.GetNode<Button>("QuitButton");

        playButton.Pressed += onPlayPressed;
        creditsButton.Pressed += onCreditsPressed;
        quitButton.Pressed += onQuitPressed;
    }

    private void onPlayPressed()
    {
        //change scene to game scene
        globals.ResetGame();
        sceneManager.ChangeScene("seasontitle", "FadeToBlack");
    }

    private void onCreditsPressed()
    {
        //show some credits here
        sceneManager.ChangeScene("credits", "FadeToBlack");
    }

    private void onQuitPressed()
    {
        //quit the game
        GetTree().Quit();
    }
}
