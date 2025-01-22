using Godot;
using System;

public partial class Editor : Control
{
	private Global _global;
	
	public override void _Ready()
	{
		_global = GetNode<Global>("/root/Global");
	}

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

	public void OnPortalGunPressed()
	{
		_global.Setting.CheckSkill(1);
	}

	public void OnBoostButtonPressed()
	{
		_global.Setting.CheckSkill(2);
	}
}
