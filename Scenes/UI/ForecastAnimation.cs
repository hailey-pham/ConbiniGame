using Godot;
using System;

public partial class ForecastAnimation : Control
{

	private AnimationPlayer _player;
	private TextureRect _rect;

	private Calendar.DisastersEnum _nextDisaster;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        _rect = GetNode<TextureRect>("TextureRect");
        _player = GetNode<AnimationPlayer>("AnimationPlayer");
        _player.Play("pulse");
    }

	public void SetDisasterType(Calendar.DisastersEnum disaster)
	{
		_nextDisaster = disaster;
		LoadDisasterForecastTexture();
	}

	public AnimationPlayer GetAnimationPlayer()
	{
		return _player;
	}

	private void LoadDisasterForecastTexture()
	{
		if (_nextDisaster != 0)
		{
			GD.Print("Disaster: " + _nextDisaster);
            var image = GD.Load<CompressedTexture2D>("res://Assets/Forecasts/" + Enum.GetName(typeof(Calendar.DisastersEnum), _nextDisaster) + "Forecast.png");

			_rect.Texture = image;
        }
	}
	
}
