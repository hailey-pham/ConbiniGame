using Godot;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public partial class SeasonTitle : Control
{
    [Export]
    private string[] SeasonTitles;
    private int seasonIdx = 0;

    private RichTextLabel seasonName;
    private AnimationPlayer animPlayer;
    private SceneManager sceneManager;
    private Calendar calendar;

    public override void _Ready()
    {
        sceneManager = GetNode<SceneManager>("/root/SceneManager");
        calendar = GetNode<Calendar>("/root/Calendar");

        seasonIdx = calendar.GetCurrentSeason() - 1;

        seasonName = GetNode<RichTextLabel>("SeasonName");
        seasonName.AppendText("[center]" + SeasonTitles[seasonIdx] + "[/center]");

        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        PlayAnim();
    }

    private async void PlayAnim()
    {
        await Task.Delay(1000);
        animPlayer.Play("print");
        await ToSignal(animPlayer, "animation_finished");
        await Task.Delay(4000);
        // check for forecast
        if (calendar.GetNextDayDisaster() != Calendar.DisastersEnum.None)
        {
            // there is disaster
            sceneManager.ChangeScene("forecastscene");
        }
        else
        {
            sceneManager.ChangeScene("gamescene", "FadeToBlack");
        }  
        
        
    }
}
