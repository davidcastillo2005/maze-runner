using Godot;
using System;

namespace MazeRunner.Scripts;

public partial class PlayerUi : Control
{
	[Export] private Player _player;
	[Export] private Label _playerNameLabel;
	[Export] private Label _skillLabel;
	[Export] private ColorRect _rect0;
	[Export] private ColorRect _rect1;
	[Export] private ColorRect _rect2;

	private int punto0;
	private int punto1;

	public override void _Ready()
	{
		_rect0.Color = new Color(0, 0, 0, 1);
		_rect1.Color = new Color(0, 0, 0, 1);
		_rect2.Color = new Color(0, 0, 0, 1);

		punto0 = (int)GetRatio(_player.BatteryLife, 10);
		punto1 = (int)GetRatio(_player.BatteryLife, 50);

		_skillLabel.Text = _player.SkillNum switch
		{
			0 => "(None)",
			1 => "(Shield)",
			2 => "(Portal)",
			3 => "(Blind)",
			4 => "(Mute)",
			5 => "(Petrify)",
			_ => throw new Exception()
		};

		_playerNameLabel.Text = _player.StrName;
	}
	public override void _Process(double delta)
	{
		if (_player.Energy < punto0)
		{
			_rect0.Color = new Color(0, 0, 0, 1);
			_rect1.Color = new Color(0, 0, 0, 1);
			_rect2.Color = new Color(0, 0, 0, 1);
		}
		else if (punto0 <= _player.Energy && _player.Energy < punto1)
		{
			_rect0.Color = new Color(1, 1, 1, 1);
			_rect1.Color = new Color(0, 0, 0, 1);
			_rect2.Color = new Color(0, 0, 0, 1);
		}
		else if (punto1 <= _player.Energy && _player.Energy < _player.BatteryLife)
		{
			_rect0.Color = new Color(1, 1, 1, 1);
			_rect1.Color = new Color(1, 1, 1, 1);
			_rect2.Color = new Color(0, 0, 0, 1);
		}
		else if (_player.Energy == _player.BatteryLife)
		{
			_rect0.Color = new Color(1, 1, 1, 1);
			_rect1.Color = new Color(1, 1, 1, 1);
			_rect2.Color = new Color(1, 1, 1, 1);
		}
	}

	private double GetRatio(float Total, float percentage)
	{
		return Total * percentage * 0.01f;
	}
}
