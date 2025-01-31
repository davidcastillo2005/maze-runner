using Godot;
using System;

namespace MazeRunner.Scripts;
public partial class GameOver : Control
{
	[Export] private Label _winningLabel;
	[Export] private Label _losingLabel;

	private Global _global;

	public override void _Ready()
	{
		_global = GetNode<Global>("/root/Global");

		if (_global.PlayerNameLost != null || _global.PlayerNameWon != null)
		{
			_winningLabel.Text = _global.PlayerNameWon;
			_losingLabel.Text = _global.PlayerNameLost;
		}
		else
		{
			throw new NullReferenceException();
		}
	}

	public override void _Process(double delta)
	{
	}

	void OnRestartButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/editor.tscn");
	}

	void OnMenuButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/menu.tscn");
	}
}
