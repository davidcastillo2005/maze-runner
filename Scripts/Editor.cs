using Godot;
using System;

public partial class Editor : Control
{
	[Export] CheckBox shieldButton;

	private Global _global;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_global = GetNode<Global>("/root/Global");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnStartButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/game.tscn");
	}

	public void OnShieldButtonPressed()
	{
		_global.Setting.CheckSkill(0);
	}
}
