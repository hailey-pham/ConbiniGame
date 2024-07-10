using Godot;
using System;

public partial class WarningAnimation : Control
{
    private AnimationPlayer _player;
    private TextureRect _rect;

    private Calendar.DisastersEnum _currentDisaster;

    public override void _Ready()
    {
        _rect = GetNode<TextureRect>("TextureRect");
        _player = GetNode<AnimationPlayer>("AnimationPlayer");
        _player.Play("pulse");
    }

    //call before entering scene tree to set disaster type
    public void SetDisasterType(Calendar.DisastersEnum disaster)
    {
        _currentDisaster = disaster;
        LoadDisasterWarningTexture();
    }

    public AnimationPlayer GetAnimationPlayer()
    {
        return _player;
    }

    private void LoadDisasterWarningTexture()
    {
            
        //if we have a disaster (which we always should)
        if (_currentDisaster != 0)
        {
            var image = GD.Load<CompressedTexture2D>("res://Assets/Warnings/" + Enum.GetName(typeof(Calendar.DisastersEnum), _currentDisaster) + "Warning.png");
            
            _rect.Texture = image;
        }
        
    }
}
