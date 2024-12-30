using Godot;
using System;

public partial class MainMenu : Control
{
	[Export] Button _playButton;
	[Export] Button _quitButton;
	private Global _global;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_playButton.ButtonDown += OnPlayButtonDown;
		_quitButton.ButtonDown += OnQuitButtonDown;
		_global = GetNode<Global>("/root/Global");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnPlayButtonDown()
	{
		_global.GoToScene("res://Scenes/game.tscn");
	}

	public void OnQuitButtonDown()
	{
		_global.QuitGame();
	}
}
