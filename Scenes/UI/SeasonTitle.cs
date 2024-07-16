using Godot;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public partial class SeasonTitle : Control
{
    [Export]
    private string[] SeasonTitles;

    [Export]
    private Color SpringColor;
    [Export]
    private Color SummerColor;
    [Export]
    private Color FallColor;
    [Export]
    private Color WinterColor;

    private List<Color> SeasonColors;
    private int seasonIdx = 0;

    private TextureRect store;
    private RichTextLabel seasonName;
    private AnimationPlayer animPlayer;
    private SceneManager sceneManager;

    public override void _Ready()
    {
        sceneManager = GetNode<SceneManager>("/root/SceneManager");

        //have to set this at runtime since we don't know what our export vars will be
        SeasonColors = new List<Color>() { SpringColor, SummerColor, FallColor, WinterColor };
        store = GetNode<TextureRect>("Store");

        store.Material.Set("shader_parameter/blendColor", SeasonColors[seasonIdx]);

        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        PlayAnim();
    }

    private async void PlayAnim()
    {
        await Task.Delay(1000);
        animPlayer.Play("print");
        await ToSignal(animPlayer, "animation_finished");
        await Task.Delay(4000);
        sceneManager.ChangeScene("gamescene", "FadeToBlack");
    }
}
