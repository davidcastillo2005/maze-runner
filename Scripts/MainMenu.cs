using Godot;
using MazeRunner.Scripts.Logic;
using System;

public partial class MainMenu : Control
{
	[Export] Button _playButton;
	[Export] Button _quitButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnPlayButtonDown()
	{
		GetTree().ChangeSceneToFile("res://Scenes/game.tscn");
	}

	public void OnQuitButtonDown()
	{
		GetTree().Quit();
	}
}
