using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;
public partial class StoreFront : Control
{
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

    // Called when the node enters the scene tree for the first time.
    private globals globals;
    private SceneManager sceneManager;
    private Calendar calendar;

    private AnimatedSprite2D store;
	public override void _Ready()
	{
		globals = GetNode<globals>("/root/Globals");

		foreach (KeyValuePair<string, Upgrade> upgrade in globals.Upgrades) {
            if (upgrade.Value.owned) {
                upgrade.Value.onLevelLoad(globals, GetNode<Node>("."));
            }
        }

        sceneManager = GetNode<SceneManager>("/root/SceneManager");
        calendar = GetNode<Calendar>("/root/Calendar");

        //have to set this at runtime since we don't know what our export vars will be
        SeasonColors = new List<Color>() { SpringColor, SummerColor, FallColor, WinterColor };

        seasonIdx = calendar.GetCurrentSeason() - 1;
        Material.Set("shader_parameter/blendColor", SeasonColors[seasonIdx]);

        store = GetNode<AnimatedSprite2D>("store");
        switch(seasonIdx)
        {
            case (0):
                store.Play("spring");
                break;
            case(1):
                store.Play("summer");
                break;
            case (2):
                store.Play("fall");
                break;
            case (3):
                store.Play("winter");
                break;
        }
        
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
